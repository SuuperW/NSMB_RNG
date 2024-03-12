import { Component, inject } from '@angular/core';
import { StepComponent } from '../step';
import { AbstractControl, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { WorkerService } from '../../worker.service';
import { RngParams } from '../../functions/rng-params-search';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { PopupDialogComponent } from '../../popup-dialog/popup-dialog.component';

@Component({
	selector: 'app-step5',
	standalone: true,
	templateUrl: './step-route.component.html',
	styleUrls: ['./step-route.component.css'],
	imports: [
		ReactiveFormsModule,
		MatDialogModule,
	],
})
export class StepRouteComponent extends StepComponent {
	worker: WorkerService = inject(WorkerService);

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
