import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StepComponent } from '../step';

@Component({
	selector: 'app-step3',
	standalone: true,
	templateUrl: './step-date.component.html',
	imports: [
		ReactiveFormsModule,
	],
})
export class StepDateComponent extends StepComponent {
	form = new FormGroup({
		dtInput: new FormControl(localStorage.getItem('date') ?? this.dateToISO(new Date()), (c: AbstractControl) => {
			if (this.form === undefined) return { err: 'not initialized' }; // this gets called once before component initialization finishes

			let dt = new Date(c.value);
			if (!isNaN(dt.valueOf())) {
				let year = dt.getFullYear();
				let maxYear = localStorage.getItem('consoleType') === '3DS' ? 2050 : 2100;
				if (year < 2000 || year >= maxYear)
					return { 'err': `Invalid date. You should not be able to set year ${year} on your DS.` };
				else {
					localStorage.setItem('date', c.value);
					return null;
				}
			} else {
				return { err: 'Enter the date on your console.' };
			}
		}),
	});

	dateToISO(date: Date): string {
		return date.toISOString().split('T')[0];
	}
}
