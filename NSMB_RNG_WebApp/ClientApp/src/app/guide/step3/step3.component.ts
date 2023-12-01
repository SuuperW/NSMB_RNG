import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StepComponent } from '../step';

@Component({
	selector: 'app-step3',
	standalone: true,
	templateUrl: './step3.component.html',
	styleUrls: ['./step3.component.css'],
	imports: [
		ReactiveFormsModule,
	],
})
export class Step3Component extends StepComponent {
	form = new FormGroup({
		dtInput: new FormControl(localStorage.getItem('datetime') ?? this.getDateTime(), (c: AbstractControl) => {
			// Do I need to check range?
			if (c.value) {
				localStorage.setItem('datetime', c.value);
				return null;
			} else {
				return { 'err': 'Enter the date and time on which RNG was initialized.' };
			}
		}),
	});

	getDateTime() {
		if (localStorage.getItem('datetime'))
			return localStorage.getItem('datetime');

		let dt = new Date();
		dt.setMinutes(dt.getMinutes() - dt.getTimezoneOffset());
		dt.setMilliseconds(0);
		return dt.toISOString().slice(0, -1);
	}
}
