import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StepComponent } from '../step';
import { TileDisplayComponent } from '../../tile-display/tile-display.component';
import { RngParams, SearchParams, SeedRow, getAllPossibleRow1 } from '../../functions/rng-params-search';
import { getRow1, getRow2 } from '../../functions/tiles';
import { SeedCalculator } from '../../seed-calculator';
import { RouterModule } from '@angular/router';
import { PrecomputedPatterns } from '../precomputed-patterns';
import { GuideComponent } from '../guide.component';

@Component({
	selector: 'app-step6',
	standalone: true,
	templateUrl: './step6.component.html',
	styleUrls: ['./step6.component.css'],
	imports: [
		ReactiveFormsModule,
		TileDisplayComponent,
		RouterModule,
	],
})
export class Step6Component extends StepComponent {
	manipDatetime: string;

	form = new FormGroup({
		row1Input: new FormControl(''),
	});
	errorStatus?= undefined;

	feedback: string = '';
	isGood: boolean = false;
	desiredRow1: string;
	desiredSeed: number;
	row2: string = '';

	patterns: PrecomputedPatterns;
	maxSubLength = 1;

	constructor(guide: GuideComponent) {
		super(guide);

		if (!this.guide.expectedParams || !this.guide.paramsRange)
			throw "invalid state: expectedParams or paramsRange is not set for step6";

		// Find all tile patterns that we should expect
		this.patterns = new PrecomputedPatterns();
		this.patterns.addParams(this.guide.paramsRange);

		// And the one we want
		let params: RngParams = this.guide.expectedParams;
		let sc = new SeedCalculator(params.mac, params.datetime, params.is3DS);
		sc.timer0 = params.timer0; sc.vCount = params.vCount;
		sc.vFrame = params.vFrame; sc.buttons = params.buttons;
		this.desiredSeed = sc.getSeed();
		this.desiredRow1 = getRow1(this.desiredSeed);

		// UI
		this.manipDatetime = `${params.datetime.toDateString()} ${params.datetime.toLocaleTimeString()}`;
	}

	private _feedbacks = [
		'This tile pattern came from RNG initializing 1 second earlier than expected. Try again.',
		'This tile pattern isn\'t what you want, but is expected and RNG was initialized at the right time. Try again.',
		'This tile pattern came from RNG initializing 1 second later than expected. Try again.',
	];
	async row1Changed(tiles: string) {
		let pi = this.patterns.getPatternInfo(tiles);
		// It is possible, though unlikely, that two pre-computed patterns share the same first row.
		// In most cases, we'll just tell the user we don't know when RNG was initialized.
		// Given the rarity of this happening, this is acceptable.
		// Except, if this is the correct pattern we absolutely need to detect that.
		if (tiles == this.desiredRow1) {
			pi = {
				ambiguous: false,
				match: { seed: this.desiredSeed, seconds: 0 }
				// If we're wrong and user doesn't have the right seed, user should see that by row2 being wrong.
			}
		}

		if (pi.match === undefined) {
			// no matching pre-calculated pattern
			if (!pi.ambiguous)
				this.feedback = 'This tile pattern doesn\'t match any expected pattern. Did you mis-type the pattern?';
			else if (tiles.length == 7)
				this.feedback = 'This tile pattern isn\'t what you want; try again. (could not determine RNG initialization time)';
			else
				this.feedback = '';
			this.isGood = false;
			this.row2 = '';
		} else {
			// Display second row so user can verify
			this.row2 = getRow2(pi.match.seed);

			// Check if it's the good pattern, +1 sec, +0 sec, or -1 sec
			this.isGood = this.desiredRow1.startsWith(tiles);
			if (this.isGood)
				this.feedback = 'This is the correct tile pattern.';
			else
				this.feedback = this._feedbacks[pi.match.seconds + 1];
		}
	}
}
