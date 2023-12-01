import { Component, ViewChild, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StepComponent } from '../step';
import { TileDisplayComponent } from '../../tile-display/tile-display.component';
import { SeedTileCalculatorService } from '../../seeds-tile-calculator.service';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { CommonModule } from '@angular/common';

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

	seeds: number[] = [];
	lastFirstRow: string = '';
	lastSecondRow: string = '';
	bottomRows: string[] = [''];
	patternIsInvalid: boolean = false;

	targetDateTime: string;

	submitCount: number = 0;

	results: any = [];

	constructor() {
		super();
		let dtStr = localStorage.getItem('datetime');
		if (dtStr) {
			let dt = new Date(dtStr);
			this.targetDateTime = dt.toLocaleString();
		} else {
			this.targetDateTime = 'INVALID [go back and enter a date and time!]';
		}

	}

	submit() {
		if (this.seeds.length == 0) {
			alert('Finish entering your tile pattern before submitting.');
			return;
		}

		const status = 'Nothing is actually happening.';
		this.addProgress(status);

		// TODO: Calculate potential RNG init params
		let userInputParams = {
			macInput: localStorage.getItem('mac'),
			consoleType: localStorage.getItem('consoleType'),
			dtInput: localStorage.getItem('datetime'),
			row1Input: this.lastFirstRow,
			row2Input: this.lastSecondRow,
		}
		let computedParams = {
			timer0: 0,
			vCount: 0,
			vFrame: 0,
		};
		let result = {
			seed: 0x0,
			result: 'mock',
		};
		this.http.post<string>('asp/seedfindingresults', {
			...userInputParams,
			...computedParams,
			...result,
		});

		this.removeProgress(status);

		this.submitCount++;
		this.results.push(computedParams);

		// If two results are positive and identical, then that's probably what we should use.
		// Well, no, we should also check that another result was positive and off by one.
		// TODO
		if (this.submitCount == 3) {
			this.errorStatus = undefined;
			alert('We have found everything we need! Go to the next step.');
		} else if (this.submitCount >= 7) {
			// Make a guess as to why we haven't gotten good results yet.
			alert('foo');
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
