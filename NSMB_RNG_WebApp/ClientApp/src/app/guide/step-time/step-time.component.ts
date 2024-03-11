import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StepComponent } from '../step';

@Component({
	selector: 'app-step-time',
	standalone: true,
	imports: [
		ReactiveFormsModule
	],
	templateUrl: './step-time.component.html',
})
export class StepTimeComponent extends StepComponent {
	form = new FormGroup({
		seconds: new FormControl(localStorage.getItem('seconds') ?? '10', (c: AbstractControl) => {
			const sec: number = parseInt(c.value);
			if (sec < 3)
				return { err: 'Enter the number of seconds between when you set the system clock and when RNG initializes.' };
			else {
				localStorage.setItem('seconds', c.value);
				return null;
			}
		}),
	});
}
