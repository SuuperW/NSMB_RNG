import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';

@Injectable({
	providedIn: 'root',
})
export class SeedTileCalculatorService {
	http: HttpClient = inject(HttpClient);
	private cache: { [key: string]: number[] | Promise<object> } = {};

	constructor() { }

	async getPossibleSeedsFor(row1: string, row2?: string) {
		let possibleSeedsRow1 = this.cache[row1];
		if (!possibleSeedsRow1) {
			possibleSeedsRow1 = this.cache[row1] = firstValueFrom(this.http.get(`asp/seeds/${row1}`)).catch(() => {
				return [];
			});
		}
		if (possibleSeedsRow1 instanceof Promise) {
			this.cache[row1] = possibleSeedsRow1 = await possibleSeedsRow1 as number[];
			console.log(`Row 1 seeds: ${possibleSeedsRow1}`);
		}
		if ((possibleSeedsRow1 as number[]).length == 0) {
			delete this.cache[row1];
			return possibleSeedsRow1;
		}

		// Check which seeds are also possible for row2
		if (!row2)
			return possibleSeedsRow1;
		return await this.findRow2Matches(possibleSeedsRow1 as number[], row2);

	}

	private TILES_PER_ROW = 27;
	private TILES_PER_SCREEN_VERTICAL = 12;
	getBottomRow(seed: number) {
		let v = seed;
		for (let i = 0; i < this.STEPS_BEFORE_ROW1 + (this.TILES_PER_ROW * (this.TILES_PER_SCREEN_VERTICAL - 1)); i++)
			v = this.nextState(v);

		let letters = ['B', 'E', 'I', 'C', 'P', 'S'];
		let tiles = '';
		for (let i = 0; i < 11; i++) {
			v = this.nextState(v);
			let tID = this.tileIDFromState(v);
			tiles += letters[tID];
		}
		return tiles;
	}

	private STEPS_BEFORE_ROW1 = 1937;
	private findRow2Matches(seedsRow1: number[], row2: string): number[] {
		let letters = [ 'B', 'E', 'I', 'C', 'P', 'S' ];
		let tiles: number[] = [];
		for (let i = 0; i < row2.length; i++) {
			let id = letters.indexOf(row2[i]);
			if (id == -1)
				return [];
			tiles.push(id);
		}

		let seedsRow2: number[] = [];
		for (let v of seedsRow1) {
			let r = v;
			for (let i = 0; i < this.STEPS_BEFORE_ROW1 + this.TILES_PER_ROW - 1; i++)
				r = this.nextState(r);

			// Check if this value matches the input for 11 tiles.
			let match = true;
			for (let i = 0; i < tiles.length; i++) {
				r = this.nextState(r);
				let tID = this.tileIDFromState(r);
				if (tID != tiles[i]) {
					match = false;
					break;
				}
			}
			if (match) {
				seedsRow2.push(...this.previousState(v));
			}

			
		}
		return seedsRow2;
	}

	private nextState(v: number) {
		// Number.MAX_SAFE_INTEGER > (0x0019660D * 0xffffffff + 0x3C6EF35F)  is true, so this should be safe.
		let a = 0x0019660D * v + 0x3C6EF35F;
		// Unfortunately, >> operator converts to 32-bit integer first so >> 32 is not helpful.
		//return a + (a >> 32);
		return (a + Math.floor(a / 0x1_0000_0000)) % 0x1_0000_0000;
	}
	// See NSMB_RNG.TilesFor12.reverseStep
	private previousState(v: number) {
		const m = 0x0019660D;
		const twoP32 = 0x1_0000_0000;

		const bigStep = Math.floor(twoP32 / m);
		const bigStepOffset = Math.floor(twoP32 - (bigStep * m));

		let tryMe = Math.floor((v - this.nextState(0)) / m);
		while (tryMe < 0x33333333) {
			let result = this.nextState(tryMe);
			// We added one so that we can easily check if r is within 1 of v.
			let diff = result + 1 - v;

			if (diff < 0) {
				// Underflow, result was less than v: Get back to >= v.
				diff += 0x1_0000_0000;
				tryMe += 1;
			}
			else if (diff <= 2)
				break;
			else {
				// Result was greater than v: Go to next value known to be before or at the next match.
				let bigStepsCount = Math.floor((diff + bigStepOffset - 1) / bigStepOffset);
				tryMe += bigStep * bigStepsCount;
			}

		}

		if (tryMe >= 0x33333333)
			return ([] as number[]);
		else {
			let allResults: number[] = [];
			let tryMeToo = tryMe;
			while (tryMeToo < 0x1_0000_0000)
			{
				if (this.nextState(tryMeToo) == v)
					allResults.push(tryMeToo);

				tryMeToo += 0x33333333;
			}
			return allResults;
		}
	}

	private tileIDFromState(v: number) {
		return ((v >> 8) & 0x7) % 6;
	}
}
