import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StepComponent } from '../step';
import { TileDisplayComponent } from '../../tile-display/tile-display.component';
import { SeedTileCalculatorService } from '../../seeds-tile-calculator.service';
import { HttpClient, HttpClientModule } from '@angular/common/http';

@Component({
	selector: 'app-step4',
	standalone: true,
	templateUrl: './step4.component.html',
	styleUrls: ['./step4.component.css'],
	imports: [
		ReactiveFormsModule,
		TileDisplayComponent,
		HttpClientModule,
	],
})
export class Step4Component implements StepComponent {
	seedService: SeedTileCalculatorService = inject(SeedTileCalculatorService);
	http: HttpClient = inject(HttpClient);

	form = new FormGroup({
		row1Input: new FormControl(''),
		row2Input: new FormControl(''),
	});
	errorStatus? = 'We don\'t have enough information yet. Continue entering tile patterns.';

	seedCandidates: number[] = [];
	seeds: number[] = [];
	lastFirstRow: string = '';
	lastSecondRow: string = '';
	bottomRow: string = '';

	targetDateTime: string;

	submitCount: number = 0;

	inProgress: boolean = false;
	status: string = '';

	results: any = [];

	constructor() {
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

		this.inProgress = true;
		this.status = 'Nothing is actually happening.';

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

		this.inProgress = false;

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
		this.seeds = this.seedCandidates = [];
		if (!tiles || tiles.length != 7)
			return;

		this.inProgress = true;
		this.status = 'Getting seed candidates from server...';
		this.seedCandidates = await this.seedService.getPossibleSeedsFor(tiles);

		this.inProgress = false;

		if (this.lastSecondRow.length == 11)
			this.row2Changed(this.lastSecondRow);
	}

	async row2Changed(tiles: string) {
		this.lastSecondRow = tiles;
		this.seeds = [];

		if (this.seedCandidates.length == 0)
			return;

		if (tiles.length == 11) {
			// TODO: Calculate seeds!
			let seed = this.seedCandidates[0];
			this.seeds.push(seed);

			// TODO: Display bottom row
		}
	}
}
