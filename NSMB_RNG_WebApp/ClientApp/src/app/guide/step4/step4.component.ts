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

type result = {
	foundParams: RngParams[],
	seeds: number[],
	row1: string,
	row2: string,
	count: number
}

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

	seeds: number[] = [];
	lastFirstRow: string = '';
	lastSecondRow: string = '';
	bottomRows: string[] = [''];
	patternIsInvalid: boolean = false;

	submitCount: number = 0;
	inProgressCount: number = 0;
	totalMatchedPatterns: number = 0;

	searchParams: SearchParams | undefined;
	results: result[] = [];
	private getAllRngParams() {
		return this.results.flatMap((result) => result.foundParams);
	}

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

		// TODO: Disable submit button?
		this.submitCount++;
		const status = 'Finding RNG initialization parameters...';
		this.addProgress(status);
		await this.processTilePattern();
		this.removeProgress(status);

		// These numbers are kinda arbitrary. The intent is to detect when something is wrong so we/user don't waste time endlessly entering bad patterns.
		const tooManyBadPatterns = () => {
			return (this.submitCount >= 3 && this.totalMatchedPatterns == 0) || (this.submitCount >= 5 && this.totalMatchedPatterns == 1);
		};
		if (tooManyBadPatterns()) {
			const statusBadTime = 'Checking +1/-1 second for all patterns... (this may take a long time)';
			this.addProgress(statusBadTime);
			let promise: Promise<boolean> = (async () => { return true })();
			for (let i = 0; i < this.results.length; i++) {
				promise = promise.then((v) => {
					return this.processTilePattern(i, -1);
				})
				.then((v) => {
					if (!v) return this.processTilePattern(i, +1);
					else return true;
				});

			}
			this.dialog.open(PopupDialogComponent, {
				data: {
					message: ['Your tile patterns aren\'t matching anything we expect.',
						'This might mean the game is initializing RNG one second earlier or later than you think.',
						'We\'re going to re-calculate with +/-1 second to see if this is the case.',
					],
				}
			});
			await promise; //? Not needed if we decide not to disable submit button
			this.removeProgress(statusBadTime);

			if (tooManyBadPatterns()) {
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

		// If two results are positive and identical, use it if another result is off by one.
		const anyOffByOne = (params: RngParams) => {
			for (let r of this.getAllRngParams()) {
				let dt = params.timer0 - r.timer0;
				let dv = params.vCount - r.vCount;
				if (dt * dv == 0 && Math.abs(dt + dv) == 1)
					return true;
			}
			return false;
		};
		let paramsToUse: RngParams | undefined;
		for (let r of this.results) {
			if (r.count > 1) {
				for (let p of r.foundParams) {
					if (anyOffByOne(p)) {
						paramsToUse = p;
						break;
					}
				}
				if (paramsToUse) break;
			}
		}
		if (paramsToUse) {
			this.errorStatus = undefined;
			localStorage.setItem('rngParams', JSON.stringify(paramsToUse));
			this.dialog.open(PopupDialogComponent, {
				data: {
					message: ['We have found everything we need! Go to the next step.'],
				}
			});
		}
	}

	// This function will look for RNG initialization parameters for the given seeds.
	// The date used is this.date plus the seconds offset.
	// It uses this.searchParams if available (and if it is available, searches +/-1 seconds automatically)
	// and updates this.searchParams according to results.
	// The results are added to this.results, or if resultId is given results are updated instead.
	private async processTilePattern(resultId: number = -1, secondsOffset: number = 0) {
		let date = this.date;
		date.setSeconds(date.getSeconds() + secondsOffset);
		let userInputParams = {
			macInput: localStorage.getItem('mac')!,
			consoleType: localStorage.getItem('consoleType'),
			row1Input: this.lastFirstRow,
			row2Input: this.lastSecondRow,
		};
		let seeds: number[];
		if (resultId != -1) {
			userInputParams.row1Input = this.results[resultId].row1;
			userInputParams.row2Input = this.results[resultId].row2;
			seeds = this.results[resultId].seeds;
		} else {
			seeds = this.seeds;
		}

		// Did we already look for rng params for this tile pattern?
		if (resultId == -1) {
			for (let r of this.results) {
				if (r.row1 == userInputParams.row1Input && r.row2 == userInputParams.row2Input) {
					r.count++;
					this.postResults(r, r.foundParams.length == 0 ? 0 : r.foundParams[0].datetime.getSeconds() - this.date.getSeconds());
					return r.foundParams.length > 0;
				}
			}
		}
		// If not, search for rng params
		this.inProgressCount++;
		// If we have previous results, base search on that.
		let rngParams: RngParams[] = [];
		let fullSearch = this.searchParams === undefined;
		if (this.searchParams) {
			rngParams = await this.worker.searchForSeeds(seeds, this.searchParams);
			if (rngParams.length == 0) {
				let o = new SearchParams(this.searchParams);
				o.datetime.setSeconds(o.datetime.getSeconds() - 1);
				rngParams = await this.worker.searchForSeeds(seeds, o);
				if (rngParams.length == 0) {
					o.datetime.setSeconds(o.datetime.getSeconds() + 2);
					rngParams = await this.worker.searchForSeeds(seeds, o);
					if (rngParams.length == 0) {
						fullSearch = true;
					}
				}
				if (resultId == -1 && rngParams.length != 0) {
					this.dialog.open(PopupDialogComponent, {
						data: {
							message: [
								`RNG was initialized at the wrong time: ${o.datetime.getTime()}.`,
								`It\'s OK this time, but for best results try to get RNG to intialize at ${this.date.getTime()} in the future.`,
							]
						}
					});
				}
			}
		}
		// Otherwise, do a full search.
		if (fullSearch) {
			rngParams = await this.worker.searchForSeeds(seeds, new SearchParams({
				mac: userInputParams.macInput,
				minTimer0: 0x300,
				maxTimer0: 0x22ff,
				is3DS: userInputParams.consoleType == '3DS',
				datetime: date,
			}));
		}
		let result = {
			foundParams: rngParams,
			seeds: seeds,
			row1: userInputParams.row1Input,
			row2: userInputParams.row2Input,
			count: 1,
		};
		if (result.foundParams.length != 0 && (resultId == -1 || this.results[resultId].foundParams.length == 0))
			this.totalMatchedPatterns++;
		if (resultId == -1) {
			this.results.push(result);
		} else if (result.foundParams.length != 0) {
			// Update, but only if we found something.
			result.count = this.results[resultId].count;
			this.results[resultId] = result;
		}

		// Set up narrower search params
		if (rngParams.length != 0) {
			// It's a list, but we don't loop through it. If there are two, one is a false positive and we can't know which.
			if (fullSearch) {
				this.searchParams = new SearchParams({
					mac: userInputParams.macInput,
					is3DS: userInputParams.consoleType == '3DS',
					datetime: date,
					minTimer0: rngParams[0].timer0 - 10,
					maxTimer0: rngParams[0].timer0 + 10,
					minVCount: rngParams[0].vCount - 3,
					maxVCount: rngParams[0].vCount + 3,
					minVFrame: rngParams[0].vFrame,
					maxVFrame: rngParams[0].vFrame,
				});
			} else if (this.searchParams) { // is never undefined, but code analysis doesn't know that
				this.searchParams.minTimer0 = Math.min(this.searchParams.minTimer0, rngParams[0].timer0 - 10);
				this.searchParams.maxTimer0 = Math.max(this.searchParams.minTimer0, rngParams[0].timer0 + 10);
				this.searchParams.minVCount = Math.min(this.searchParams.minVCount!, rngParams[0].vCount - 3);
				this.searchParams.maxVCount = Math.max(this.searchParams.maxVCount!, rngParams[0].vCount + 3);
			}
		}

		this.inProgressCount--;
		this.postResults(result, secondsOffset);
		return result.foundParams.length > 0;
	}

	private postResults(results: result, offsetSeconds: number) {
		// The default behaviour for Date values is to convert them to UTC. We do not want that, we want to ignore timezones entirely.
		let dtStr: Date | string = this.date;
		dtStr.setMinutes(dtStr.getMinutes() - dtStr.getTimezoneOffset());
		dtStr = dtStr.toISOString().slice(0, -1);

		let postData: any = {
			...results,
			datetime: dtStr,
			gameVersion: localStorage.getItem('gameVersion'),
			mac: localStorage.getItem('mac'),
			is3DS: localStorage.getItem('consoleType') == '3DS',
			offsetUsed: offsetSeconds,
		};
		this.http.post<string>('asp/submitResults', postData).subscribe(); // need to subscribe or it won't actually send the request?
	}

	async row1Changed(tiles: string) {
		this.lastFirstRow = tiles;
		this.seeds = [];
		if (!tiles || tiles.length != 7)
			return;

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

		if (this.lastSecondRow.length == 11)
			this.row2Changed(this.lastSecondRow);
	}

	r2cc = 0;
	async row2Changed(tiles: string) {
		this.patternIsInvalid = false;
		this.lastSecondRow = tiles;
		this.seeds = [];
		this.bottomRows = [''];
		let r2cc = ++this.r2cc;

		if (tiles.length == 11 && this.lastFirstRow.length == 7) {
			const status = "Finding seeds...";
			this.addProgress(status);
			this.seeds = await this.seedService.getPossibleSeedsFor(this.lastFirstRow, tiles) as number[];
			this.removeProgress(status);

			if (r2cc != this.r2cc)
				return; // User triggered this method again before getPossibleSeedsFor returned.

			if (this.seeds.length == 0) {
				this.patternIsInvalid = true;
				return;
			}

			let rows: Set<string> = new Set();
			this.bottomRows = [];
			for (let seed of this.seeds)
				rows.add(this.seedService.getBottomRow(seed));
			for (let row of rows)
				this.bottomRows.push(row);
		}
	}
}
