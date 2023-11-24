import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';

@Injectable({
	providedIn: 'root',
})
export class SeedsByRow1Service {
	http: HttpClient = inject(HttpClient);

	constructor() { }

	async getPossibleSeedsFor(row1: string) {
		let temp: number[] = [];

		let result = await firstValueFrom(this.http.get(`asp/seeds/${row1}`));
		console.log(result);

		return result;
	}
}
