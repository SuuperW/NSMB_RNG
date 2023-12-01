import { Component } from '@angular/core';
import { StepComponent } from '../step';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
	selector: 'app-step2',
	standalone: true,
	templateUrl: './step2.component.html',
	styleUrls: ['./step2.component.css'],
	imports: [
		ReactiveFormsModule,
	],
})
export class Step2Component extends StepComponent {
	form = new FormGroup({
		mac: new FormControl(localStorage.getItem('mac') ?? '00:11:22:aa:bb:cc', (c: AbstractControl<string>) => {
			if (c.value.match(/([\dabcdef]{2}:){5}[\dabcdef]{2}/i) || c.value.match(/[\dabcdef]{12}/i)) {
				localStorage.setItem('mac', c.value);
				return null;
			}
			else
				return { err: 'MAC address is invalid' };
		}),

	});
}
