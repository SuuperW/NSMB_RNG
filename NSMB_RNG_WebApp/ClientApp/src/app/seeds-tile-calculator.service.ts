import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';

@Injectable({
	providedIn: 'root',
})
export class SeedTileCalculatorService {
	http: HttpClient = inject(HttpClient);
	private cache: { [key: string]: number[] } = {};

	constructor() { }

	async getPossibleSeedsFor(row1: string) {
		let result = this.cache[row1];
		if (!result) {
			result = await firstValueFrom(this.http.get(`asp/seeds/${row1}`)) as number[];
			this.cache[row1] = result;
		}

		return result;
	}
}
