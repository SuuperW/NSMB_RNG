import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StepComponent } from '../step';
import { TileDisplayComponent } from '../../tile-display/tile-display.component';
import { SeedTileCalculatorService } from '../../seeds-tile-calculator.service';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { RngParams, SearchParams, searchForSeeds } from '../../functions/rng-params-search';

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
	],
})
export class Step4Component extends StepComponent {
	seedService: SeedTileCalculatorService = inject(SeedTileCalculatorService);
	http: HttpClient = inject(HttpClient);

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
	results: { foundParams: RngParams[], row1: string, row2: string, count: number }[] = [];
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

	submit() {
		if (this.seeds.length == 0) {
			alert('Finish entering your tile pattern before submitting.');
			return;
		}

		this.submitCount++;
		const status = 'Finding RNG initialization parameters...';
		this.addProgress(status);

		// Did we already look for rng params for this tile pattern?
		let alreadySearched = false;
		for (let r of this.results) {
			if (r.row1 == this.lastFirstRow && r.row2 == this.lastSecondRow) {
				alreadySearched = true;
				r.count++;
				break;
			}
		}
		// If not, search for rng params
		if (!alreadySearched) {
			this.inProgressCount++;
			let userInputParams = {
				macInput: localStorage.getItem('mac')!,
				consoleType: localStorage.getItem('consoleType'),
				dtInput: this.date,
				row1Input: this.lastFirstRow,
				row2Input: this.lastSecondRow,
			};
			// If we have previous results, base search on that.
			let rngParams: RngParams[] = [];
			let fullSearch = this.searchParams === undefined;
			if (this.searchParams) {
				rngParams = searchForSeeds(this.seeds, this.searchParams);
				if (rngParams.length == 0)
					fullSearch = true;
			}
			// Otherwise, do a full search.
			if (fullSearch) {
				rngParams = searchForSeeds(this.seeds, {
					mac: userInputParams.macInput,
					minTimer0: 0x300,
					maxTimer0: 0x22ff,
					is3DS: userInputParams.consoleType == '3DS',
					datetime: this.date,
				});
			}
			let result = {
				foundParams: rngParams,
				row1: userInputParams.row1Input,
				row2: userInputParams.row2Input,
				count: 1,
			};
			if (result.foundParams.length != 0)
				this.totalMatchedPatterns++;
			this.results.push(result);
			this.http.post<string>('asp/seedfindingresults', result);
			this.inProgressCount--;

			// Set up narrower search params
			if (rngParams.length != 0) {
				if (fullSearch) {
					this.searchParams = {
						mac: userInputParams.macInput,
						is3DS: userInputParams.consoleType == '3DS',
						datetime: this.date,
						minTimer0: rngParams[0].timer0 - 10,
						maxTimer0: rngParams[0].timer0 + 10,
						minVCount: rngParams[0].vCount - 3,
						maxVCount: rngParams[0].vCount + 3,
						minVFrame: rngParams[0].vFrame,
						maxVFrame: rngParams[0].vFrame,
					};
				} else if (this.searchParams) { // is never undefined, but code analysis doesn't know that
					this.searchParams.minTimer0 = Math.min(this.searchParams.minTimer0, rngParams[0].timer0 - 10);
					this.searchParams.maxTimer0 = Math.max(this.searchParams.minTimer0, rngParams[0].timer0 + 10);
					this.searchParams.minVCount = Math.min(this.searchParams.minVCount!, rngParams[0].vCount - 3);
					this.searchParams.maxVCount = Math.max(this.searchParams.maxVCount!, rngParams[0].vCount + 3);
				}
			}
		}
		this.removeProgress(status);

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
			alert('We have found everything we need! Go to the next step.');
		} else {
			// TODO: Offer choice to search +/-1 second.
			if ((this.submitCount >= 3 && this.totalMatchedPatterns == 0) || (this.submitCount >= 5 && this.totalMatchedPatterns == 1)) {
				alert('Your tile patterns aren\'t matching anything we expect. This could indicate that you have \
incorrectly entered something, such as mac address or date. Or it might mean you\'re not following the correct \
process to collect tile patterns, or the number of seconds is incorrect. Or, there might just be something \
that is affecting RNG that I don\'t know about.');
				// TODO: Make and refer to detailed instructions + video.
			}
		}
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
			alert('Failed to get seed candidates from server. Please try again (by backspacing one tile for first row and re-entering it).');
			return;
		}

		if (this.lastSecondRow.length == 11)
			this.row2Changed(this.lastSecondRow);
	}

	async row2Changed(tiles: string) {
		this.patternIsInvalid = false;
		this.lastSecondRow = tiles;
		this.seeds = [];
		this.bottomRows = [''];

		if (tiles.length == 11 && this.lastFirstRow.length == 7) {
			const status = "Finding seeds...";
			this.addProgress(status);
			this.seeds = await this.seedService.getPossibleSeedsFor(this.lastFirstRow, tiles) as number[];
			this.removeProgress(status);

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
