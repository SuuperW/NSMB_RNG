import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { WorkerService } from './worker-wrapper';

import * as rng from './functions/rng';

@Injectable({
	providedIn: 'root',
})
export class SeedTileCalculatorService {
	http: HttpClient = inject(HttpClient);
	worker: WorkerService = inject(WorkerService);
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
		}
		if ((possibleSeedsRow1 as number[]).length == 0) {
			delete this.cache[row1];
			return possibleSeedsRow1;
		}

		// Check which seeds are also possible for row2
		if (!row2)
			return possibleSeedsRow1;
		return await this.worker.findRow2Matches(possibleSeedsRow1, row2);
	}

	private TILES_PER_ROW = 27;
	private TILES_PER_SCREEN_VERTICAL = 12;
	private STEPS_BEFORE_ROW1 = 1937;
	getBottomRow(seed: number) {
		let v = seed;
		for (let i = 0; i < this.STEPS_BEFORE_ROW1 + (this.TILES_PER_ROW * (this.TILES_PER_SCREEN_VERTICAL - 1)); i++)
			v = rng.nextState(v);

		let letters = ['B', 'E', 'I', 'C', 'P', 'S'];
		let tiles = '';
		for (let i = 0; i < 11; i++) {
			v = rng.nextState(v);
			let tID = rng.tileIDFromState(v);
			tiles += letters[tID];
		}
		return tiles;
	}

}
