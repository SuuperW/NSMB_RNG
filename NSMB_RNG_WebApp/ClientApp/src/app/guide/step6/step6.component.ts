import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StepComponent } from '../step';
import { TileDisplayComponent } from '../../tile-display/tile-display.component';
import { RngParams, SearchParams, SeedRow, getAllPossibleRow1 } from '../../functions/rng-params-search';
import { getRow1, getRow2 } from '../../functions/tiles';
import { SeedCalculator } from '../../seed-calculator';
import { RouterModule } from '@angular/router';

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
	manipDatetime: Date = new Date(localStorage.getItem('manipDatetime')!);

	form = new FormGroup({
		row1Input: new FormControl(''),
	});
	errorStatus?= undefined;

	feedback: string = '';
	isGood: boolean = false;
	desiredRow1: string;
	row2: string = '';

	patternMap: { [key: string]: SeedRow[] };
	expectedPatterns: Set<SeedRow>[];
	allSubs: Set<string>[];
	maxSubLength = 1;

	constructor() {
		super();
		let params: RngParams = JSON.parse(localStorage.getItem('rngParams')!);
		params.datetime = this.manipDatetime;

		// Find all tile patterns that we should expect
		let sc = new SeedCalculator(params.mac, params.datetime, params.is3DS);
		sc.timer0 = params.timer0; sc.vCount = params.vCount;
		sc.vFrame = params.vFrame; sc.buttons = params.buttons;
		this.desiredRow1 = getRow1(sc.getSeed());
		let searchParams = new SearchParams({
			mac: localStorage.getItem('mac')!,
			is3DS: localStorage.getItem('consoleType') == '3DS',
			datetime: new Date(params.datetime),
			minTimer0: params.timer0 - 10, // Few if any consoles actually have this wide a range
			maxTimer0: params.timer0 + 10,
			minVCount: params.vCount - 3, // I'm not sure if any consoles have a range of more than +/-1
			maxVCount: params.vCount + 3,
			minVFrame: params.vFrame,
			maxVFrame: params.vFrame,
		});
		this.expectedPatterns = [];
		this.expectedPatterns[0] = new Set(getAllPossibleRow1(searchParams));
		searchParams.datetime.setSeconds(searchParams.datetime.getSeconds() - 1);
		this.expectedPatterns[-1] = new Set(getAllPossibleRow1(searchParams));
		searchParams.datetime.setSeconds(searchParams.datetime.getSeconds() + 2);
		this.expectedPatterns[+1] = new Set(getAllPossibleRow1(searchParams));

		// Process patterns for use in row1Changed
		this.allSubs = [];
		let map: { [key: string]: SeedRow[] } = {};
		let add = (iter: Iterable<SeedRow>, count: number) => {
			if (count > this.maxSubLength) {
				this.maxSubLength = count;
				this.allSubs[count] = new Set<string>();
			}
			for (let sr of iter) {
				let sub = sr.pattern.substring(0, count);
				if (!map[sub]) map[sub] = [];
				map[sub].push(sr);
				this.allSubs[count].add(sub);
			}
		}

		let expanded = false;
		add(this.expectedPatterns[0], 2);
		add(this.expectedPatterns[-1], 2);
		add(this.expectedPatterns[+1], 2);
		while (!expanded) {
			expanded = true;
			for (let sub in map) {
				if (map[sub].length > 1 && sub.length < 7) {
					add(map[sub], sub.length + 1);
					delete map[sub];
					expanded = false;
				}
			}
		}

		// It is possible, though unlikely, that two pre-computed patterns share the same first row.
		// In most cases, this shouldn't be much of an issue. The user would see an incorrect second row
		// and most likely just assume something weird is happening or there's a bug, and just move on.
		// Given the rarity of this, we'll ignore the problem for all except one case:
		// If the desired pattern (that signals correct RNG) shares its first row with another.
		if (map[this.desiredRow1] && map[this.desiredRow1].length > 1) {
			// find the one we want and remove all others
			let seed = sc.getSeed();
			let index = map[this.desiredRow1].findIndex((v) => v.seed == seed);
			map[this.desiredRow1].splice(index, 1);
		}

		this.patternMap = map;
	}

	async row1Changed(tiles: string) {
		// Look for a matching pre-calculated pattern.
		let sr: SeedRow | undefined = this.patternMap[tiles] ? this.patternMap[tiles][0] : undefined;
		let wasMax = tiles.length >= this.maxSubLength;
		let sub = tiles;
		while (!sr && sub.length > 2) {
			sub = sub.substring(0, sub.length - 1);
			sr = this.patternMap[sub] ? this.patternMap[sub][0] : undefined;
		}
		if (sr && !sr.pattern.startsWith(tiles))
			sr = undefined;
		if (!sr) {
			// no matching pre-calculated pattern
			if (wasMax || (this.allSubs[tiles.length] && !this.allSubs[tiles.length].has(tiles)))
				this.feedback = 'This tile pattern doesn\'t match any expected pattern. Did you mis-type the pattern?';
			else
				this.feedback = '';
			this.isGood = false;
			this.row2 = '';
			return;
		}

		// Display second row so user can verify
		this.row2 = getRow2(sr.seed);

		// Check if it's the good pattern, +1 sec, +0 sec, or -1 sec
		this.isGood = this.desiredRow1.startsWith(tiles);
		if (this.isGood)
			this.feedback = 'This is the correct tile pattern.';
		else if (this.expectedPatterns[0].has(sr))
			this.feedback = 'This tile pattern isn\'t what you want, but is expected and RNG was initialized at the right time. Try again.';
		else if (this.expectedPatterns[-1].has(sr))
			this.feedback = 'This tile pattern came from RNG initializing 1 second earlier than expected. Try again.';
		else if (this.expectedPatterns[+1].has(sr))
			this.feedback = 'This tile pattern came from RNG initializing 1 second later than expected. Try again.';
	}
}
