import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { StepComponent } from '../step';
import { TileDisplayComponent } from '../../tile-display/tile-display.component';
import { SeedTileCalculatorService } from '../../seeds-tile-calculator.service';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { RngParams, SearchParams } from '../../functions/rng-params-search';
import { WorkerService } from '../../worker.service';
import { PopupDialogComponent } from '../../popup-dialog/popup-dialog.component';
import { PrecomputedPatterns } from '../precomputed-patterns';
import { getRow1, getRow2 } from '../../functions/tiles';
import { RngParamsSearch, RngParamsSearchResultManager, SearchInputs } from '../rng-params-search-result-manager';

type ProcessingInputs = {
	row1: string,
	row2: string,
	seeds: number[],
	mac: string,
	consoleType: string,
	date: Date,
	isRedo?: boolean,
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
	templateUrl: './step4.component.html',
	styleUrls: ['./step4.component.css'],
	imports: [
		ReactiveFormsModule,
		TileDisplayComponent,
		HttpClientModule,
		CommonModule,
		MatDialogModule,
	],
})
export class Step4Component extends StepComponent {
	seedService: SeedTileCalculatorService = inject(SeedTileCalculatorService);
	worker: WorkerService = inject(WorkerService);
	http: HttpClient = inject(HttpClient);
	dialog: MatDialog = inject(MatDialog);

	form = new FormGroup({
		row1Input: new FormControl(''),
		row2Input: new FormControl(''),
	});
	errorStatus?= 'We don\'t have enough information yet. Continue entering tile patterns.';

	targetDateTime: string;
	date: Date;
	generalInputs: { mac: string, consoleType: string, date: Date }

	seeds: number[] = [];
	lastFirstRow: string = '';
	lastSecondRow: string = '';
	bottomRows: string[] = [''];
	patternIsInvalid: boolean = false;
	computingLastRow: boolean = false;

	inProgressCount: number = 0;

	knownPatterns: PrecomputedPatterns;
	knownSearchParams: SearchParams[];
	requiredFullSearch: boolean = false;

	resultManager: RngParamsSearchResultManager;

	constructor() {
		super();
		let dtStr = localStorage.getItem('datetime');
		if (dtStr) {
			this.date = new Date(dtStr);
			this.targetDateTime = this.date.toLocaleString();
		} else {
			this.date = new Date();
			this.targetDateTime = 'INVALID [go back and enter a date and time!]';
		}
		this.resultManager = new RngParamsSearchResultManager(this.date, this.http);

		let is3DS = localStorage.getItem('consoleType') == '3DS';
		let basicParams = new SearchParams({
			mac: localStorage.getItem('mac')!,
			is3DS: is3DS,
			datetime: this.date,
			buttons: 0,
			minTimer0: 0,
			maxTimer0: 0,
		});
		this.knownPatterns = new PrecomputedPatterns();
		this.knownSearchParams = [];
		for (let kp of knownParams) {
			let sp = new SearchParams(basicParams);
			sp.minTimer0 = kp[0];
			sp.maxTimer0 = kp[1];
			sp.minVCount = kp[2];
			sp.maxVCount = kp[3];
			sp.minVFrame = sp.maxVFrame = kp[4];
			this.knownPatterns.addParams(sp, 1);
			this.knownSearchParams.push(sp);
		}

		this.generalInputs = {
			mac: localStorage.getItem('mac')!,
			consoleType: localStorage.getItem('consoleType')!,
			date: new Date(this.date),
		}
	}

	async submit() {
		if (this.seeds.length == 0) {
			this.dialog.open(PopupDialogComponent, {
				data: {
					message: ['Finish entering your tile pattern before submitting.'],
				}
			});
			return;
		}

		let pi = this.getProcessingInputs();
		this.form.controls.row1Input.setValue('');
		this.form.controls.row2Input.setValue('');

		const status = 'Finding RNG initialization parameters...';
		this.addProgress(status);
		let anyFound = this.resultManager.totalMatchedPatterns > 0;
		let result = await this.processTilePattern(pi);
		if (!anyFound && result) {
			// On the first success, go back to failed patterns to check +/-1 second
			for (let i of this.resultManager.getInputsWithNoFoundRngParams()) {
				await this.processTilePattern(this.getProcessingInputs(i), 0, false);
			}
		}
		this.removeProgress(status);

		if (this.resultManager.suspectUserErrorOrStrangeConsole()) {
			const statusBadTime = 'Checking +1/-1 second for all patterns... (this may take a long time)';
			this.addProgress(statusBadTime);
			let promise: Promise<boolean> = (async () => { return true })();
			for (let i of this.resultManager.getInputsWithNoFoundRngParams()) {
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
				this.dialog.open(PopupDialogComponent, {
					data: {
						message: ['Unfortunately, we didn\'t find anything useful.',
							'This might mean you incorrectly entered something. For example, you might have a typo in your mac address.',
							'Or it might mean you\'re not following the correct process to collect tile patterns.', // TODO: Make and refer to detailed instructions + video.
							'Or, there might be something that is affecting RNG that I don\'t know about. ' +
							'If you need help, record a video of you viewing your MAC address, setting date and time, and getting a tile ' +
							'pattern on your console. Send the video to @suuper on Discord and I\'ll see if I can figure out what the problem is.'
						],
					}
				});
			} else {
				this.dialog.open(PopupDialogComponent, {
					data: {
						message: ['Good news! We found something potentially useful. Keep going.']
					}
				});
			}
		}

		let paramsToUse = this.resultManager.getMostLikelyResult();
		if (paramsToUse) {
			let message: string[]
			// If the user keeps getting the same pattern over and over, and it looks good, and it isn't from previously known RNG params.
			if (this.resultManager.getDistinctPatternCount() == 1 && this.requiredFullSearch && this.resultManager.submitCount === 4) {
				message = ['You got the same tiles four times in a row. This is unusual: normally RNG is not this consistent.',
					'We did find RNG params, and you may proceed to the next step. However, the results have a slight chance of being wrong.',
					'You may want to try to get another pattern. Going back and using a different date or time might help. Getting two patterns would allow cross-referencing the results to verify they are correct. If you choose to continue and try to get another pattern, you will be alerted if the RNG params already found can be verified correct.',
				];
			} else {
				message = ['We have found everything we need! Go to the next step.'];
			}

			this.dialog.open(PopupDialogComponent, {
				data: { message: message }
			});
			this.errorStatus = undefined;
			localStorage.setItem('rngParams', JSON.stringify(paramsToUse));
		}
	}

	private getProcessingInputs(result?: SearchInputs): ProcessingInputs {
		let processingInputs: ProcessingInputs = {
			...this.generalInputs,
			row1: result?.row1 ?? this.lastFirstRow,
			row2: result?.row2 ?? this.lastSecondRow,
			seeds: result?.seeds ?? this.seeds,
		};
		if (result)
			processingInputs.isRedo = true;

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

		// Did we already look for rng params for this tile pattern?
		if (!processingInptus.isRedo) {
			let existing = this.resultManager.getFor(processingInptus.row1, processingInptus.row2);
			if (existing !== null) {
				this.resultManager.submitResult(existing);
				return existing.result.length > 0;
			}
		}

		// If not, search for rng params
		this.inProgressCount++;
		let rngParams: RngParams[] = [];
		if (secondsOffset == 0) {
			// If we have previous results, base search on that.
			// If not, use known params from other consoles.
			let searchParams = this.resultManager.getSearchParams();
			for (let sp of searchParams ? [searchParams] : this.knownSearchParams) {
				[rngParams, secondsOffset] = await this.searchPlusMinusOneSecond(processingInptus.seeds, sp);
				if (rngParams.length != 0)
					break;
			}
			if (secondsOffset != 0 && !processingInptus.isRedo) {
				this.dialog.open(PopupDialogComponent, {
					data: {
						message: [
							`RNG was initialized 1 second too ${secondsOffset == 1 ? 'late' : 'early'}.`,
							`It\'s OK this time, but for best results try to get RNG to intialize at ${this.targetDateTime} in the future.`,
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

		this.resultManager.submitResult({
			result: rngParams,
			seeds: processingInptus.seeds,
			row1: processingInptus.row1,
			row2: processingInptus.row2,
			offsetUsed: secondsOffset,
		});

		this.inProgressCount--;
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


	private _changedByUser = true;
	private _row2SetByAutocomplete = false;
	async row1Changed(tiles: string) {
		this.lastFirstRow = tiles;
		this.seeds = [];

		// Auto-complete
		let precomputedResult = this.knownPatterns.getPatternInfo(tiles);
		if (!precomputedResult.ambiguous && precomputedResult.match && precomputedResult.extraTiles! > 0) {
			// We only do the auto-completion if the user has entered 1 more tile than we need for a match.
			// This is to reduce the confusion of having lots of bad autocompletes when the users rng params aren't known.

			this._row2SetByAutocomplete = true;
			let row2 = getRow2(precomputedResult.match.seed);
			// _changedByUser being false will short-circuit the event handler.
			// However, the event handler does not happen until later on so we cannot
			// un-set _changedByUser afterwards. The handler must do that for us.
			// Additionally, the event handler will only be triggered if the set value is different from the current value.
			if (row2 != this.form.value.row2Input) this._changedByUser = false;
			this.form.controls.row2Input.setValue(row2);
			this.setSeeds([precomputedResult.match.seed]);
			this.lastFirstRow = getRow1(precomputedResult.match.seed);
		} else if (this._row2SetByAutocomplete) {
			this.form.controls.row2Input.setValue('');
			this._row2SetByAutocomplete = false;
		}

		if (tiles.length != 7) {
			return;
		}

		const status = 'Getting seed candidates from server...';
		this.addProgress(status);
		let result = await this.seedService.getPossibleSeedsFor(tiles);
		this.removeProgress(status);
		if (result.length == 0) {
			this.dialog.open(PopupDialogComponent, {
				data: {
					message: ['Failed to get seed candidates from server. Please try again (by backspacing one tile for first row and re-entering it).']
				}
			});
			return;
		}

		if (this.lastSecondRow.length == 11 && !this._row2SetByAutocomplete)
			this.row2Changed(this.lastSecondRow);
	}

	r2cc = 0;
	async row2Changed(tiles: string) {
		this.patternIsInvalid = false;
		this.lastSecondRow = tiles;
		let r2cc = ++this.r2cc;

		if (!this._changedByUser) {
			// Angular will not call this handler at the time the value is set. The handler will get called later.
			// Thus, the caller cannot handle setting _changedByUser back to true. We must do it here.
			this._changedByUser = true;
			return;
		}

		this.seeds = [];
		this.bottomRows = [''];
		this._row2SetByAutocomplete = false;

		if (tiles.length == 11 && this.lastFirstRow.length == 7) {
			const status = "Finding seeds...";
			this.computingLastRow = true;
			this.addProgress(status);
			let seeds = await this.seedService.getPossibleSeedsFor(this.lastFirstRow, tiles) as number[];
			this.removeProgress(status);

			if (r2cc != this.r2cc)
				return; // User triggered this method again before getPossibleSeedsFor returned.

			if (seeds.length == 0)
				this.patternIsInvalid = true;
			this.setSeeds(seeds);
		}

		this.computingLastRow = false;
	}

	setSeeds(seeds: number[]) {
		this.seeds = seeds;

		let rows: Set<string> = new Set();
		this.bottomRows = [];
		for (let seed of this.seeds)
			rows.add(this.seedService.getBottomRow(seed));
		for (let row of rows)
			this.bottomRows.push(row);
	}

	tileClick(letter: string) {
		let control = this.form.controls.row1Input;
		if ((this.form.value.row1Input?.length ?? 0) >= 7) {
			control = this.form.controls.row2Input;
		}
		control.setValue((control.value ?? '') + letter);
	}
}
