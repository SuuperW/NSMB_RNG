import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StepComponent } from '../step';
import { TileDisplayComponent } from '../../tile-display/tile-display.component';
import { RngParams, SearchParams, SeedRow, getAllPossibleRow1 } from '../../functions/rng-params-search';
import { getRow1, getRow2 } from '../../functions/tiles';
import { SeedCalculator } from '../../seed-calculator';
import { RouterModule } from '@angular/router';
import { PrecomputedPatterns } from '../precomputed-patterns';

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

	constructor() {
		super();

		let params: RngParams = JSON.parse(localStorage.getItem('rngParams')!);
		let date = new Date(localStorage.getItem('manipDatetime')!);
		params.datetime = date;
		this.manipDatetime = `${date.toDateString()} ${date.toLocaleTimeString()}`;

		// Find all tile patterns that we should expect
		this.patterns = new PrecomputedPatterns();
		this.patterns.addParams(new SearchParams({
			mac: localStorage.getItem('mac')!,
			is3DS: localStorage.getItem('consoleType') == '3DS',
			datetime: new Date(params.datetime),
			minTimer0: params.timer0 - 10, // Few if any consoles actually have this wide a range
			maxTimer0: params.timer0 + 10,
			minVCount: params.vCount - 3, // I'm not sure if any consoles have a range of more than +/-1
			maxVCount: params.vCount + 3,
			minVFrame: params.vFrame,
			maxVFrame: params.vFrame,
		}));
		let sc = new SeedCalculator(params.mac, params.datetime, params.is3DS);
		sc.timer0 = params.timer0; sc.vCount = params.vCount;
		sc.vFrame = params.vFrame; sc.buttons = params.buttons;
		this.desiredSeed = sc.getSeed();
		this.desiredRow1 = getRow1(this.desiredSeed);
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
