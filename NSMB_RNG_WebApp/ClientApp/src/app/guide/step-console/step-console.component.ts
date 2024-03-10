import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StepComponent } from '../step';

@Component({
	selector: 'app-step1',
	standalone: true,
	templateUrl: './step-console.component.html',
	styleUrls: ['./step-console.component.css'],
	imports: [
		ReactiveFormsModule,
	],
})
export class StepConsoleComponent extends StepComponent {
	form = new FormGroup({
		consoleType: new FormControl(localStorage.getItem('consoleType'), (c: AbstractControl<string>) => {
			if (c.value) {
				localStorage.setItem('consoleType', c.value);
				return null;
			} else {
				return { 'err': 'Select a console type.' };
			}
		}),
		gameVersion: new FormControl(localStorage.getItem('gameVersion'), (c: AbstractControl<string>) => {
			if (c.value) {
				localStorage.setItem('gameVersion', c.value);
				return null;
			} else {
				return { 'err': 'Select a game version.' };
			}
		}),
	});
}
