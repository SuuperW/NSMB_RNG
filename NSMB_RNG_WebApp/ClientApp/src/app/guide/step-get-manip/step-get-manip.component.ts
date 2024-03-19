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
	selector: 'app-step-get-manip',
	standalone: true,
	templateUrl: './step-get-manip.component.html',
	imports: [
		ReactiveFormsModule,
		RouterModule,
		MatDialogModule,
		FullPatternInputComponent,
		ClickableTilesComponent,
	],
})
export class StepGetManipComponent extends StepComponent implements AfterViewInit {
	dialog: MatDialog = inject(MatDialog);
	timeFinder: TimeFinderService = inject(TimeFinderService);

	manipDatetime: string = '[calculating date and time...]';
	datetimeCalculated: boolean = false;

	form = new FormGroup({});
	errorStatus?= undefined;

	@ViewChild(FullPatternInputComponent) patternInput: FullPatternInputComponent = null!;

	feedback: string = '';
	isGood: boolean = false;
	desiredRow1: string = '';

	protected newParams?: RngParams;
	private lastMatchInfo: PatternMatchInfo | null = null;

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
			this.setTargetDateTime(value);
			this.datetimeCalculated = true;
		});
	}

	setTargetDateTime(dt: Date) {
		if (isNaN(dt.valueOf())) {
			// This shouldn't ever happen.
			this.manipDatetime = '[ERROR]';
			return;
		}
		if (!this.guide.expectedParams || !this.guide.paramsRange) return; // won't ever happen

		this.guide.expectedParams.datetime = dt;
		this.guide.paramsRange.datetime = dt;
		this.newParams = undefined;

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
		// When the page is translated by Chrome, text is replaced by the translation.
		// This destroys the Angular {{ data binding }}. In the case of this.feedback this is OK since it only has one value that is ever displayed.
		// However if this is changed and feedback can take one of multiple values, we will need to address this issue.
		// A solution might be to use an array and *ngFor, since that seems to work for guide component's status messages.

		this.feedback = '';
		this.lastMatchInfo = pi;

		// It is possible, though exceedingly unlikely, that two pre-computed patterns share the same first two rows.
		// We ignore this possibility. (It cannot happen for rows that look like the correct pattern.)

		if (pi?.match === undefined) {
			// no matching pre-calculated pattern
			if (!pi?.ambiguous)
				this.feedback = 'This tile pattern doesn\'t match any expected pattern. Did you mis-type the pattern?';
		}

		this.cdr.detectChanges();
	}

	/**
	 * Checks how the user's tile pattern compares to expected RNG and provides appropriate feedback to the user.
	 * Tracks submissions with resultManager to determine if a new datetime should be used.
	 */
	protected async submit() {
		if (this.lastMatchInfo?.match) {
			// Check if it's the good pattern
			let row1 = this.patternInput.getPattern()[0];
			this.isGood = this.desiredRow1.startsWith(row1);
			// Display message for the number of seconds
			let message: string[];
			if (this.isGood) {
				message = [
					'This is the correct tile pattern. Yay!',
					'Don\'t turn off the game now. For further instructions, see text below the submit button.',
				];
			} else {
				let newTime = new Date(this.guide.expectedParams!.datetime);
				newTime.setSeconds(newTime.getSeconds() + this.lastMatchInfo.match.seconds);
				message = [this._feedbacks[this.lastMatchInfo.match.seconds + 1].replace('{t}', newTime.toLocaleTimeString())];
			}
			this.dialog.open(PopupDialogComponent, {
				data: { message }
			});
			// Submit the result and check if suggested RNG params has changed.
			const rngParams = searchForSeeds([this.lastMatchInfo.match.seed], this.guide.paramsRange!);
			if (rngParams.length !== 1) {
				throw 'Something went wrong and we couldn\'t determine RNG params. (this should never happen)';
			}
			this.guide.resultManager = this.guide.resultManager!; // We know it is defined here.
			const oldParams = this.guide.resultManager.getMostLikelyResult()!;
			this.guide.resultManager.submitResult({
				result: rngParams,
				seeds: [this.lastMatchInfo.match.seed],
				row1: this.patternInput.getPattern()[0],
				row2: this.patternInput.getPattern()[1],
				offsetUsed: 0,
			});
			this.patternInput.clear();
			const newParams = this.guide.resultManager.getMostLikelyResult()!;
			const paramsEqual = (p1: RngParams, p2: RngParams) => { return p1.timer0 === p2.timer0 && p1.vCount === p2.vCount; };
			if (!paramsEqual(oldParams, newParams)) {
				// Since the most likely / recommended params have changed, find the corresponding datetime.
				const status = 'Searching for new date and time...';
				this.addProgress(status);

				const route = localStorage.getItem('route') ?? 'normal';
				this.timeFinder.addParams(newParams, route);
				const dt = await this.timeFinder.getTime(newParams, route)!;

				// Ensure this is still the most likely params; finding the new time could have taken a long time.
				if (paramsEqual(this.guide.resultManager.getMostLikelyResult()!, newParams)) {
					this.newParams = newParams;
					this.newParams.datetime = dt;
				}

				this.removeProgress(status);
			}
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

	protected async useNewParams() {
		this.guide.expectedParams = this.newParams;
		this.setTargetDateTime(this.newParams!.datetime);

		this.dialog.open(PopupDialogComponent, {
			data: { message: [`The new date and time is ${this.manipDatetime}`] }
		});		
	}
}
