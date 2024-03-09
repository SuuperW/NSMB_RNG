import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StepComponent } from '../step';
import { TileDisplayComponent } from '../../tile-display/tile-display.component';
import { RngParams, searchForSeeds } from '../../functions/rng-params-search';
import { getRow1, getRow2 } from '../../functions/tiles';
import { SeedCalculator } from '../../seed-calculator';
import { RouterModule } from '@angular/router';
import { PrecomputedPatterns } from '../precomputed-patterns';
import { GuideComponent } from '../guide.component';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { PopupDialogComponent } from '../../popup-dialog/popup-dialog.component';

@Component({
	selector: 'app-step6',
	standalone: true,
	templateUrl: './step6.component.html',
	styleUrls: ['./step6.component.css'],
	imports: [
		ReactiveFormsModule,
		TileDisplayComponent,
		RouterModule,
		MatDialogModule,
	],
})
export class Step6Component extends StepComponent {
	dialog: MatDialog = inject(MatDialog);

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

	retrySeed: number = -1;
	offerRetry: boolean = false;

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
		'RNG initialized at {t}, 1 second too early. Try again.',
		'This tile pattern isn\'t what you want, but is expected and RNG was initialized at the right time. Try again.',
		'RNG initialized at {t}, 1 second too late. Try again.',
	];
	async row1Changed(tiles: string) {
		this.retrySeed = -1;
		this.offerRetry = false;

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
			else {
				let newTime = new Date(this.guide.expectedParams!.datetime);
				newTime.setSeconds(newTime.getSeconds() + pi.match.seconds);
				this.feedback = this._feedbacks[pi.match.seconds + 1].replace('{t}', newTime.toLocaleTimeString());

				// Originally I was going to only suggest getting a new date/time after getting the same
				// wrong seed 3+ times. But I think that is likely to not work since users may decide
				// not to re-enter the same incorrect tile pattern when they've seen it before.
				// Because why would they? They already know it's wrong.
				if (pi.match.seconds === 0)
					this.retrySeed = pi.match.seed;
			}
		}
	}

	tileClick(letter: string) {
		let control = this.form.controls.row1Input;
		control.setValue((control.value ?? '') + letter);
	}
	backspace() {
		let control = this.form.controls.row1Input;
		control.setValue(control.value?.substring(0, control.value.length - 1) ?? '');
	}
	clearTiles() {
		this.form.controls.row1Input.setValue('');
	}

	calculateNewTime() {
		// get RNG params for new seed
		let rngParams = searchForSeeds([this.retrySeed], this.guide.paramsRange!);
		if (rngParams.length !== 1) {
			this.dialog.open(PopupDialogComponent, {
				data: {
					message: ['Something went wrong and we couldn\'t determine RNG params. (this should never happen)'],
				}
			});
		}

		// use them and go back
		this.guide.expectedParams = rngParams[0];
		this.guide.previous();
	}
}
