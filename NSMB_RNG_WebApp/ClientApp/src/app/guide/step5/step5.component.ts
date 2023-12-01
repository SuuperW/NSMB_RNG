import { Component } from '@angular/core';
import { StepComponent } from '../step';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
	selector: 'app-step5',
	standalone: true,
	templateUrl: './step5.component.html',
	styleUrls: ['./step5.component.css'],
	imports: [
		ReactiveFormsModule,
	],
})
export class Step5Component extends StepComponent {
	form = new FormGroup({
		route: new FormControl('', (c: AbstractControl<string>) => {
			if (c.value) {
				this.addProgress('');
				// TODO: Actually do the calculations.
				setTimeout(() => {
					this.errorStatus = undefined;
					this.removeProgress('');
					localStorage.setItem('manipDatetime', localStorage.getItem('datetime')!);
					localStorage.setItem('route', c.value);
				}, 10_000);
				return null;
			} else {
				return { 'err': 'Select a route.' };
			}
		}),
	});
	errorStatus? = 'Plase wait';
}
