import { AfterViewInit, ChangeDetectorRef, Component, ViewChild, inject } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StepComponent } from '../step';
import { RngParams, searchForSeeds } from '../../functions/rng-params-search';
import { getRow1 } from '../../functions/tiles';
import { SeedCalculator } from '../../seed-calculator';
import { RouterModule } from '@angular/router';
import { PatternMatchInfo, PrecomputedPatterns } from '../precomputed-patterns';
import { GuideComponent } from '../guide.component';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { PopupDialogComponent } from '../../popup-dialog/popup-dialog.component';
import { FullPatternInputComponent } from 'src/app/tile-display/full-pattern-input.component';
import { ClickableTilesComponent } from 'src/app/tile-display/clickable-tiles.component';
import { TimeFinderService } from 'src/app/time-finder.service';

@Component({
	selector: 'app-step6',
	standalone: true,
	templateUrl: './step6.component.html',
	imports: [
		ReactiveFormsModule,
		RouterModule,
		MatDialogModule,
		FullPatternInputComponent,
		ClickableTilesComponent,
	],
})
export class Step6Component extends StepComponent implements AfterViewInit {
	dialog: MatDialog = inject(MatDialog);
	timeFinder: TimeFinderService = inject(TimeFinderService);

	manipDatetime: string = '[calculating date and time...]';
	datetimeCalculated: boolean = false;

	form = new FormGroup({});
	errorStatus?= undefined;

	@ViewChild(FullPatternInputComponent) patternInput: FullPatternInputComponent = null!;

	feedback: string = '';
	isGood: boolean = false;
	secondsDelta?: number;
	desiredRow1: string = '';

	maxSubLength = 1;

	retrySeed: number = -1;
	offerRetry: boolean = false;

	constructor(guide: GuideComponent, private cdr: ChangeDetectorRef) {
		super(guide);
	}

	ngAfterViewInit(): void {
		if (!this.guide.expectedParams || !this.guide.paramsRange)
			throw "invalid state: expectedParams or paramsRange is not set for step6";

		let dt = this.timeFinder.getTime(this.guide.expectedParams, localStorage.getItem('route')!);
		if (dt === undefined)
			throw 'error: no datetime'; // should never happen

		this.patternInput.patternChanged.add(this.patternChanged.bind(this));

		let status = 'Searching for a date and time... (may take a few minutes)';
		this.addProgress(status);
		dt.then((value) => {
			this.removeProgress(status);

			if (isNaN(value.valueOf())) {
				// This shouldn't ever happen.
				this.manipDatetime = '[ERROR]';
				return;
			}

			this.setTargetDateTime(value);
			this.datetimeCalculated = true;
		});
	}

	setTargetDateTime(dt: Date) {
		if (!this.guide.expectedParams || !this.guide.paramsRange) return; // won't ever happen
		this.guide.expectedParams.datetime = dt;
		this.guide.paramsRange.datetime = dt;

		// Find all tile patterns that we should expect
		this.patternInput.autocomplete = new PrecomputedPatterns();
		this.patternInput.autocomplete.addParams(this.guide.paramsRange!);

		// And the one we want
		let params: RngParams = this.guide.expectedParams!;
		let sc = new SeedCalculator(params.mac, params.datetime, params.is3DS);
		sc.timer0 = params.timer0; sc.vCount = params.vCount;
		sc.vFrame = params.vFrame; sc.buttons = params.buttons;
		let desiredSeed = sc.getSeed();
		this.desiredRow1 = getRow1(desiredSeed);

		// UI
		this.manipDatetime = `${params.datetime.toDateString()} ${params.datetime.toLocaleTimeString()}`;
	}

	private _feedbacks = [
		'RNG initialized at {t}, 1 second too early. Try again.',
		'This tile pattern isn\'t what you want, but is expected and RNG was initialized at the right time. Try again.',
		'RNG initialized at {t}, 1 second too late. Try again.',
	];
	patternChanged(pi: PatternMatchInfo | null) {
		this.retrySeed = -1;
		this.offerRetry = false;
		this.feedback = '';
		this.secondsDelta = undefined;

		// It is possible, though exceedingly unlikely, that two pre-computed patterns share the same first two rows.
		// We ignore this possibility. (It cannot happen for rows that look like the correct pattern.)

		if (pi?.match === undefined) {
			// no matching pre-calculated pattern
			if (!pi?.ambiguous)
				this.feedback = 'This tile pattern doesn\'t match any expected pattern. Did you mis-type the pattern?';
			this.isGood = false;
		} else {
			// Check if it's the good pattern
			let row1 = this.patternInput.getPattern()[0];
			this.isGood = this.desiredRow1.startsWith(row1);
			if (this.isGood)
				this.feedback = 'This is the correct tile pattern.';

			this.secondsDelta = pi.match.seconds;
		}

		//this.cdr.detectChanges();
	}

	protected submit() {
		if (this.secondsDelta) { // Autocomplete was triggered with a match
			// Display message for the number of seconds
			let message: string[];
			if (this.isGood) {
				message = [
					this.feedback,
					'You\'re all set to begin speedrun attempts!',
					'Go to the game\'s main menu and start a new game. You won\'t need to set the clock again until you close the game.',
					'Notice how most tiles are the same, and the pattern repeats; it should be easy to check for this pattern in the future without coming back here.',
					'Review instructions for how to manipulate RNG during attempts <a [routerLink]="[\'/in-run\']">here</a>.'
				];
			} else {
				let newTime = new Date(this.guide.expectedParams!.datetime);
				newTime.setSeconds(newTime.getSeconds() + this.secondsDelta);
				message = [this._feedbacks[this.secondsDelta + 1].replace('{t}', newTime.toLocaleTimeString())];
			}
			this.dialog.open(PopupDialogComponent, {
				data: { message }
			});
			// Submit to the results manager, which will track most common RNG params.
			let rngParams = searchForSeeds([this.retrySeed], this.guide.paramsRange!);
			if (rngParams.length !== 1) {
				throw 'Something went wrong and we couldn\'t determine RNG params. (this should never happen)';
			}
			// TODO: Get a reference to result manager, and modify it to accept same-time submissions (or have a mode to do so)
			// Check if suggested RNG params has changed.
		} else {
			// tell user to finish entering tile pattern or re-affirm that it "doesn't match any expected pattern"
			if (this.feedback === '') {
				this.dialog.open(PopupDialogComponent, {
					data: {
						message: ['Finish entering your tile pattern before submitting.'],
					}
				});
			} else {
				this.dialog.open(PopupDialogComponent, {
					data: {
						message: ['This tile pattern doesn\'t match any expected pattern. Did you mis-type the pattern?'],
					}
				});
			}
		}
	}

	tileClick(letter: string) {
		this.patternInput.appendTile(letter);	
	}
	backspace() {
		// TODO
	}
	clearTiles() {
		this.patternInput.clear();
	}

	async calculateNewTime() {
		// get RNG params for new seed
		let rngParams = searchForSeeds([this.retrySeed], this.guide.paramsRange!);
		if (rngParams.length !== 1) {
			this.dialog.open(PopupDialogComponent, {
				data: {
					message: ['Something went wrong and we couldn\'t determine RNG params. (this should never happen)'],
				}
			});
		}

		// use them and get new time
		this.guide.expectedParams = rngParams[0];
		let status = 'Searching for a date and time... (may take a few minutes)';
		this.addProgress(status);
		let dt = await this.timeFinder.getTime(this.guide.expectedParams, localStorage.getItem('route') ?? 'normal');
		this.removeProgress(status);
		if (!dt || isNaN(dt.valueOf())) {
			// This shouldn't ever happen.
			this.manipDatetime = '[ERROR]';
			return;
		}
		this.setTargetDateTime(dt);
	}
}
