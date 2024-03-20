import { Component } from '@angular/core';
import { StepComponent } from '../step';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
	selector: 'app-step5',
	standalone: true,
	templateUrl: './step-route.component.html',
	styleUrls: ['./step-route.component.css'],
	imports: [
		ReactiveFormsModule,
	],
})
export class StepRouteComponent extends StepComponent {
	form = new FormGroup({
		route: new FormControl(localStorage.getItem('route') ?? '', (c: AbstractControl<string>) => {
			if (c.value) {
				localStorage.setItem('route', c.value);
				return null;
			} else {
				return { 'err': 'Select a route.' };
			}
		}),
	});
}
