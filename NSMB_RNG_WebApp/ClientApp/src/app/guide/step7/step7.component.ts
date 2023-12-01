import { Component } from '@angular/core';
import { StepComponent } from '../step';
import { FormGroup, ReactiveFormsModule } from '@angular/forms';

@Component({
	selector: 'app-step7',
	standalone: true,
	templateUrl: './step7.component.html',
	styleUrls: ['./step7.component.css'],
	imports: [
		ReactiveFormsModule,
	],
})
export class Step7Component extends StepComponent {
	form = new FormGroup({});
}
