import { Component } from '@angular/core';
import { StepComponent } from '../step';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
	selector: 'app-step0',
	standalone: true,
	templateUrl: './step0.component.html',
	styleUrls: ['./step0.component.css'],
	imports: [
		ReactiveFormsModule,
	],
})
export class Step0Component extends StepComponent {
	form = new FormGroup({});
}
