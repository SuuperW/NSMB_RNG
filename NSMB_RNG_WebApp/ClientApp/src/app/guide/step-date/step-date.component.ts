import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StepComponent } from '../step';

@Component({
	selector: 'app-step3',
	standalone: true,
	templateUrl: './step-date.component.html',
	styleUrls: ['./step-date.component.css'],
	imports: [
		ReactiveFormsModule,
	],
})
export class StepDateComponent extends StepComponent {
	form = new FormGroup({
		dtInput: new FormControl(localStorage.getItem('datetime') ?? this.dateTimeToString(null), (c: AbstractControl) => {
			if (this.form === undefined) return { err: 'not initialized' }; // this gets called once before component initialization finishes

			let dt = new Date(c.value);
			// Some browsers (especially mobile) do not support settings the seconds of a datetime element via the native UI.
			// If the user entered 0 seconds, that is probably why. Display buttons to increment/decrement the seconds.
			if (dt.getSeconds() === 0) {
				this.unhideStepperButtons();
				// Try to minimize number of clicks the user will need.
				dt.setSeconds(10);
				c.setValue(this.dateTimeToString(dt));
			}

			if (!isNaN(dt.getHours())) {
				let year = dt.getFullYear();
				let maxYear = localStorage.getItem('consoleType') === '3DS' ? 2050 : 2100;
				if (year < 2000 || year >= maxYear)
					return { 'err': `Invalid date. You should not be able to set year ${year} on your DS.` };
				else {
					localStorage.setItem('datetime', c.value);
					this.guide.targetDate = dt;
					return null;
				}
			} else {
				return { err: 'Enter the date and time on which RNG was initialized.' };
			}
		}),
	});

	private unhideStepperButtons() {
		let buttons = document.querySelectorAll('.stepperButton');
		buttons.forEach((b) => { // regular for loop syntax doesn't work??
			b.classList.remove('hide');
		});
	}

	step(n: number) {
		let dtInput = document.querySelector('#dtInput') as HTMLInputElement;
		if (n === 1)
			dtInput.stepUp();
		else
			dtInput.stepDown();
		// Angular's ReactiveFormsModule does not work correctly when using stepUp and stepDown.
		this.form.controls.dtInput.setValue(dtInput.value);
	}

	dateTimeToString(input: string | Date | null) {
		if (input === null && localStorage.getItem('datetime'))
			return localStorage.getItem('datetime')!;

		if (input === null)
			input = new Date();
		if (typeof input === 'string')
			input = new Date(input);

		input.setMinutes(input.getMinutes() - input.getTimezoneOffset());
		input.setMilliseconds(0);
		return input.toISOString().slice(0, -1);
	}
}
