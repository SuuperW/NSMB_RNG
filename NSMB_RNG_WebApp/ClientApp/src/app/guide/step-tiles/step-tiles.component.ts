import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { AfterViewInit, Component, ViewChild, inject } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { GuideComponent } from '../guide.component';
import { PrecomputedPatterns } from '../precomputed-patterns';
import { RngParamsSearchResultManager, SearchInputs } from '../rng-params-search-result-manager';
import { StepComponent } from '../step';
import { RngParams, SearchParams } from 'src/app/functions/rng-params-search';
import { PopupDialogComponent } from 'src/app/popup-dialog/popup-dialog.component';
import { SeedTileCalculatorService } from 'src/app/seeds-tile-calculator.service';
import { FullPatternInputComponent } from 'src/app/tile-display/full-pattern-input.component';
import { ClickableTilesComponent } from 'src/app/tile-display/clickable-tiles.component';
import { TimeFinderService } from 'src/app/time-finder.service';
import { WorkerService } from 'src/app/worker.service';

type ProcessingInputs = {
	row1: string,
	row2: string,
	seeds: number[],
	mac: string,
	consoleType: string,
	date: Date,
}

let knownParams = [
	// timer0 min, max, vCount min, max, vFrame
	[0x566, 0x567, 0x26, 0x27, 5], // DSi, 3DS
	[0x564, 0x565, 0xe8, 0xe8, 6], // DSLite
	[0x556, 0x557, 0x38, 0x39, 8], // DSPhat
	[0x20ca, 0x20ca, 0xa, 0xb, 6], // 3DS, non-US
	[0x20a8, 0x20a8, 0x26, 0x27, 5], // 3DS
	[0x56c, 0x56c, 0x18, 0x18, 5], // 3DS
	//[0x566, 0x567, 0x9, 0xa, 6],
]

@Component({
	selector: 'app-step4',
	standalone: true,
	templateUrl: './step-tiles.component.html',
	styleUrls: ['./step-tiles.component.css'],
	imports: [
		ReactiveFormsModule,
		ClickableTilesComponent,
		FullPatternInputComponent,
		HttpClientModule,
		CommonModule,
		MatDialogModule,
	],
})
export class StepTilesComponent extends StepComponent implements AfterViewInit {
	seedService: SeedTileCalculatorService = inject(SeedTileCalculatorService);
	worker: WorkerService = inject(WorkerService);
	timeFinder: TimeFinderService = inject(TimeFinderService);
	http: HttpClient = inject(HttpClient);
	dialog: MatDialog = inject(MatDialog);

	form = new FormGroup({});
	errorStatus?= 'We don\'t have enough information yet. Continue entering tile patterns.';

	targetDateTime: string;
	date: Date;
	generalInputs: { mac: string, consoleType: string }
	route: string;

	@ViewChild(FullPatternInputComponent) patternInput: FullPatternInputComponent = null!;

	submitCount: number = 0; // the result manager also tracks this, but we need the UI to update right away and not wait for that
	private _inProgressCount: number = 0;
	private get inProgressCount() { return this._inProgressCount; }
	private set inProgressCount(v: number) {
		this._inProgressCount = v;
		if (v === 0)
			this.timeFinder.resumeSearch();
		else
			this.timeFinder.pauseSearch();
	}

	knownSearchParams: SearchParams[] = [];
	requiredFullSearch: boolean = false;
	displayedBadPatternsMessage: boolean = false;
	displayedPotentiallyUseful: boolean = false;
	incorrectPatternsMessage: string[] = [
		'This might mean you incorrectly entered something. For example, you might have a typo in your mac address.',
		'Or it might mean you\'re not following the correct process to collect tile patterns. (See https://youtu.be/4XJ8dgqii10)',
		'Or, there might be something that is affecting RNG that I don\'t know about. ' +
		'If you need help, record a video of you viewing your MAC address, setting date and time, and getting a tile ' +
		'pattern on your console. Send the video to @suuper on Discord and I\'ll see if I can figure out what the problem is.'
	];

	public get resultManager() { return this.guide.resultManager!; } // We set it in constructor, and nothing else sets it; it is always defined.

	constructor(guide: GuideComponent) {
		super(guide);

		this.route = localStorage.getItem('route') ?? 'normal';
		let dateStr = localStorage.getItem('date');
		let secStr = localStorage.getItem('seconds');
		if (dateStr && secStr) {
			let sec = parseInt(secStr);
			this.date = new Date(`${dateStr} 01:45:00`);
			this.date.setSeconds(sec);
			this.targetDateTime = this.date.toLocaleString();
			this.timeFinder.setBaseSeconds(sec)
		} else {
			this.date = new Date();
			this.targetDateTime = 'INVALID [go back and enter a date and time!]';
		}
		this.guide.resultManager = new RngParamsSearchResultManager(this.date, this.http);

		this.generalInputs = {
			mac: localStorage.getItem('mac')!,
			consoleType: localStorage.getItem('consoleType')!,
		}
	}

	ngAfterViewInit(): void {
		// We can autocomplete based on known parameters from other consoles.
		// However this list is very incomplete. To avoid confusion of autocompleting incorrectly
		// we require 1 extra tile. This decreases the chance of identifying an autocompletion
		// that doesn't match the user's tiles.
		this.patternInput.autocompleteRequireExtraTiles = 1;
		this.updateAutocomplete();	
	}

	private updateAutocomplete() {
		let sp = this.resultManager.getSearchParams(this.date);
		if (sp) {
			this.patternInput.autocomplete = new PrecomputedPatterns();
			this.patternInput.autocomplete.addParams(sp, 1);
			// We stop requiring 1 extra tile before autocompleting when we have params from user patterns.
			this.patternInput.autocompleteRequireExtraTiles = 0;
			return;
		}

		// Until we find RNG params for this user, auto-complete will be based on RNG params for all consoles.
		let is3DS = this.generalInputs.consoleType == '3DS';
		let basicParams = new SearchParams({
			mac: this.generalInputs.mac,
			is3DS: is3DS,
			datetime: new Date(this.date),
			buttons: 0,
			minTimer0: 0,
			maxTimer0: 0,
		});

		this.patternInput.autocomplete = new PrecomputedPatterns();
		let firstTime = this.knownSearchParams.length === 0;
		for (let kp of knownParams) {
			let sp = new SearchParams(basicParams);
			sp.minTimer0 = kp[0];
			sp.maxTimer0 = kp[1];
			sp.minVCount = kp[2];
			sp.maxVCount = kp[3];
			sp.minVFrame = sp.maxVFrame = kp[4];
			this.patternInput.autocomplete.addParams(sp, 1);
			if (firstTime)
				this.knownSearchParams.push(sp);
		}
	}

	dateToTime(date: Date): string {
		return `${date.getHours()}:${date.getMinutes().toString().padStart(2, '0')}`;
	}

	async submit() {
		if (this.patternInput.seeds.length == 0) {
			this.dialog.open(PopupDialogComponent, {
				data: {
					message: ['Finish entering your tile pattern before submitting.'],
				}
			});
			return;
		}
		this.submitCount++;

		// We clear the tile pattern input, but first record them so they aren't lost before processing.
		let pi = this.getProcessingInputs();
		this.patternInput.clear();

		// Update time and auto-complete list for next tile pattern
		this.date.setMinutes(this.date.getMinutes() - 1);
		this.targetDateTime = this.date.toLocaleString();
		this.updateAutocomplete();

		// This is important. User must not miss this fact.
		if (this.resultManager.submitCount === 0) {
			this.dialog.open(PopupDialogComponent, {
				data: {
					message: [
						`The time you need to set is now ${this.dateToTime(this.date)}. It will change every time you submit a tile pattern!`,
					],
				}
			});
		}

		let oldRange = this.resultManager.getSearchParams(new Date());

		// Process this tile pattern submission
		const status = 'Finding RNG initialization parameters...';
		this.addProgress(status);
		let anyFound = this.resultManager.distinctParamsCount > 0;
		let result = await this.processTilePattern(pi);
		if (!anyFound && result) {
			// On the first success, go back to failed patterns to check +/-1 second
			for (let i of this.resultManager.emptySearches) {
				await this.processTilePattern(this.getProcessingInputs(i), 0, false);
			}
		}
		this.removeProgress(status);

		let message: string[] | undefined = undefined;
		if (this.resultManager.suspectUserErrorOrStrangeConsole() && !this.displayedBadPatternsMessage) {
			this.displayedBadPatternsMessage = true;
			if (this.displayedPotentiallyUseful) {
				message = ['Your tile patterns aren\'t consistently matching any expectations.', ...this.incorrectPatternsMessage];
			} else {
				const statusBadTime = 'Checking +1/-1 second for all patterns... (this may take a long time)';
				this.addProgress(statusBadTime);
				let promise: Promise<boolean> = (async () => { return true })();
				for (let i of this.resultManager.emptySearches) {
					promise = promise.then((v) => {
						return this.processTilePattern(this.getProcessingInputs(i), -1);
					})
						.then((v) => {
							if (!v) return this.processTilePattern(this.getProcessingInputs(i), +1);
							else return true;
						});

				}
				this.dialog.open(PopupDialogComponent, {
					data: {
						message: ['We haven\'t been able to find RNG params that match your RNG values/tile patterns.',
							'This might mean the game is initializing RNG one second earlier or later than you think.',
							'We\'re going to re-calculate with +/-1 second to see if this is the case.',
						],
					}
				});
				await promise;
				this.removeProgress(statusBadTime);

				if (this.resultManager.suspectUserErrorOrStrangeConsole()) {
					message = ['Unfortunately, we didn\'t find anything useful.', ...this.incorrectPatternsMessage];
				} else {
					message = ['Good news! We found something potentially useful. Keep going.'];
					this.displayedPotentiallyUseful = true;
					this.displayedBadPatternsMessage = false;
				}
			}

		}

		// See if we know which RNG params to use, and potentially tell the user something.
		let paramsToUse = this.resultManager.getMostLikelyResult();
		if (paramsToUse) {
			message = ['We have found everything we need! Go to the next step.'];

			this.errorStatus = undefined;
			this.guide.paramsRange = this.resultManager.getSearchParams(new Date())!;
			this.guide.expectedParams = paramsToUse;
		}
		if (message) {
			this.dialog.open(PopupDialogComponent, {
				data: { message: message }
			});
		}

		// Update auto-complete list (again) if search range has changed
		let newRange = this.resultManager.getSearchParams(new Date());
		if (oldRange && newRange && (oldRange.minTimer0 != newRange.minTimer0 || oldRange.minVCount != newRange.minVCount))
		this.updateAutocomplete();
	}

	private getProcessingInputs(result?: SearchInputs): ProcessingInputs {
		let rows = this.patternInput.getPattern();
		let processingInputs: ProcessingInputs = {
			...this.generalInputs,
			date: result?.time ?? this.date,
			row1: result?.row1 ?? rows[0],
			row2: result?.row2 ?? rows[1],
			seeds: result?.seeds ?? this.patternInput.seeds,
		};

		// clone Date object
		processingInputs.date = new Date(processingInputs.date);
		return processingInputs;
	}

	// This function will look for RNG initialization parameters for the given seeds.
	// It uses this.searchParams if available (and if it is available, searches +/-1 seconds automatically)
	// and updates this.searchParams according to results.
	// The results are added to this.results, or if resultId is given results are updated instead.
	private async processTilePattern(processingInptus: ProcessingInputs,
		secondsOffset = 0, // How many seconds from the target time to search with.
		allowFull = true, // Set to false to only do the quick search with this.searchParams.
	) {
		processingInptus.date.setSeconds(processingInptus.date.getSeconds() + secondsOffset);

		// If not, search for rng params
		this.inProgressCount++;
		let rngParams: RngParams[] = [];
		if (secondsOffset == 0) {
			let searchParams = this.resultManager.getSearchParams(processingInptus.date);
			// If we have previous results, base search on that.
			// If not, use known params from other consoles.
			for (let sp of searchParams ? [searchParams] : this.knownSearchParams) {
				sp.datetime = processingInptus.date; // knownSearchParams date will not be up-to-date (and cannot be, sinec it is possible to be processing two patterns concurrently)
				[rngParams, secondsOffset] = await this.searchPlusMinusOneSecond(processingInptus.seeds, sp);
				if (rngParams.length != 0)
					break;
			}
			if (secondsOffset != 0 && allowFull) { // allowFull: This will be false if the pattern being processed is not the most recently entered pattern. (in which case showing this message would confuse the user)
				this.dialog.open(PopupDialogComponent, {
					data: {
						message: [
							`RNG was initialized at ${rngParams[0].datetime.toLocaleTimeString()}, 1 second too ${secondsOffset == 1 ? 'late' : 'early'}.`,
							`It\'s OK this time (because the app detected it), but for best results try to get RNG to intialize at the correct time in the future.`,
						]
					}
				});
			}
		}

		// We should do a full search if no results were found by prior searches.
		if (rngParams.length == 0 && allowFull) {
			this.requiredFullSearch = true;
			rngParams = await this.worker.searchForSeeds(processingInptus.seeds, new SearchParams({
				mac: processingInptus.mac,
				minTimer0: 0x300,
				maxTimer0: 0x22ff,
				is3DS: processingInptus.consoleType == '3DS',
				datetime: processingInptus.date,
			}));
		}

		// There's no sense re-submitting a failed retry search
		if (allowFull || rngParams.length !== 0) {
			this.resultManager.submitResult({
				result: rngParams,
				seeds: processingInptus.seeds,
				row1: processingInptus.row1,
				row2: processingInptus.row2,
				offsetUsed: secondsOffset,
			}, processingInptus.date);
		}

		this.inProgressCount--;

		for (let params of rngParams)
			this.timeFinder.addParams(params, this.route);

		return rngParams.length > 0;
	}
	private async searchPlusMinusOneSecond(seeds: number[], searchParams: SearchParams): Promise<[RngParams[], number]> {
		let offsets = [0, -1, +1];
		for (let offset of offsets) {
			// set time
			let o = new SearchParams(searchParams);
			o.datetime.setSeconds(o.datetime.getSeconds() + offset);
			// search
			let rngParams = await this.worker.searchForSeeds(seeds, o);
			if (rngParams.length > 0) {
				return [rngParams, offset];
			}
		}
		
		return [[], 0];
	}

	tileClick(letter: string) {
		this.patternInput.appendTile(letter);
	}

	backspace() {
		this.patternInput.backspace();
	}
}
