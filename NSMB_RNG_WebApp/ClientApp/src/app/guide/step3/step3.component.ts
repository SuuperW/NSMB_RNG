import { Component } from '@angular/core';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';
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
export class Step3Component implements StepComponent {
	form = new FormGroup({});
}
