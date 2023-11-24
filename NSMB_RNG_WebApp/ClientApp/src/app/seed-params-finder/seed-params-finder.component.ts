import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TileDisplayComponent } from '../tile-display/tile-display.component';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators, ValidatorFn } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { inject } from '@angular/core';

@Component({
	selector: 'app-seed-params-finder',
	standalone: true,
	imports: [
		CommonModule,
		TileDisplayComponent,
		ReactiveFormsModule,
		HttpClientModule,
	],
	templateUrl: './seed-params-finder.component.html',
	styleUrls: [ './seed-params-finder.component.css' ],
})
export class SeedParamsFinderComponent {
	// TODO: validate more
	inputsForm = new FormGroup({
		macInput: new FormControl(localStorage.getItem('mac') ?? '00:11:22:aa:bb:cc', (c: AbstractControl<any, any>) => {
			let macStr: string = c.value;
			if (macStr.match(/([\dabcdef]{2}:){5}[\dabcdef]{2}/i) || macStr.match(/[\dabcdef]{12}/i))
				return null;
			else
				return { err: 'MAC address is invalid' };
		}),
		consoleType: new FormControl(localStorage.getItem('consoleType') ?? ''),
		dtInput: new FormControl(this.getDateTime()),
		row1Input: new FormControl(''),
		row2Input: new FormControl(''),
	});
	httpClient: HttpClient = inject(HttpClient);

	getDateTime() {
		if (localStorage.getItem('datetime'))
			return localStorage.getItem('datetime');

		let dt = new Date();
		dt.setMinutes(dt.getMinutes() - dt.getTimezoneOffset());
		dt.setMilliseconds(0);
		return dt.toISOString().slice(0, -1);
	}

	constructor() {

	}

	macChanged() {
		if (this.inputsForm.controls.macInput.valid)
			localStorage.setItem('mac', this.inputsForm.value.macInput ?? '');
	}

	formSubmit() {
		if (!this.inputsForm.valid) {
			// TODO: Show more helpful messages
			alert('some input is not valid');
			return;
		}

		// TODO: Get list of possible seed values for the given top row.
		// TODO: Calculate the RNG seed value based on above list.
		// TODO: Calculate SeedParams based on seed value and user inputs.

		let result: string = 'test';
		this.httpClient.post<string>('asp/seedfindingresults', {
			...this.inputsForm.value,
			seed: 0x0,
			result: result,
		}).subscribe({ next: (v) => console.log(v) });
	}
}
