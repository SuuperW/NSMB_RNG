import { AfterViewInit, ChangeDetectorRef, Component, ViewChild, inject } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { ComponentContainer } from '../component-container';
import { StepComponent } from './step';
import { Step1Component } from './step1/step1.component';
import { Step2Component } from './step2/step2.component';
import { Step3Component } from './step3/step3.component';
import { Step4Component } from './step4/step4.component';
import { Step5Component } from './step5/step5.component';
import { Step6Component } from './step6/step6.component';
import { CommonModule } from '@angular/common';
import { Step0Component } from './step0/step0.component';
import { PopupDialogComponent } from '../popup-dialog/popup-dialog.component';

interface TI {
	new(): StepComponent
}

@Component({
	selector: 'app-guide',
	standalone: true,
	templateUrl: './guide.component.html',
	styleUrls: ['./guide.component.css'],
	imports: [		
		Step1Component,
		ComponentContainer,
		CommonModule,
		MatDialogModule,
	],
})
export class GuideComponent implements AfterViewInit {
	stepComponentList: TI[] = [
		Step0Component, Step1Component, Step2Component, Step3Component, Step4Component,
		Step5Component, Step6Component,
	];

	dialog: MatDialog = inject(MatDialog);

	@ViewChild(ComponentContainer<StepComponent>) stepContainer!: ComponentContainer<StepComponent>;
	get stepComponent() {
		return this.stepContainer?.component;
	}

	currentStep: number = 0;

	isInProgress: boolean = false;
	progressStatus: string[] = [];

	constructor(private cdr: ChangeDetectorRef) { }

	onLoadStepComponent(component: StepComponent) {
		let ths = this;
		component.progress.add((progressStatus: string[]) => {
			ths.isInProgress = progressStatus.length != 0;
			ths.progressStatus = progressStatus.slice();
			// Since we're updating proerties outside of events that Angular expects to modify this component, we need to manually call detectChanges.
			ths.cdr.detectChanges();
		});
		
	}

	ngAfterViewInit() {
		this.stepContainer.componentCreated.add(this.onLoadStepComponent.bind(this));
		if (this.stepContainer.component)
			this.onLoadStepComponent(this.stepContainer.component);
	}

	next() {
		if (this.stepComponent?.form.invalid || this.stepComponent?.errorStatus) {
			this.dialog.open(PopupDialogComponent, {
				data: {
					message: [this.getErrorsForForm(this.stepComponent?.form, this.stepComponent?.errorStatus)]
				}
			});
			return;
		}

		if (this.currentStep != this.stepComponentList.length - 1)
			this.currentStep++;
	}

	previous() {
		if (this.currentStep != 0)
			this.currentStep--;
	}

	getErrorsForForm(form: FormGroup, otherError?: string) {
		let sb = [];
		if (otherError)
			sb.push(otherError);
		// I wish the form's .errors would include errors from all controls. (though Idk how it'd handle conflicting keys...)
		for (let c in form.controls) {
			let control = form.controls[c];
			for (let k in control.errors) {
				sb.push(control.errors[k]);
			}
		}
		return sb.join('\n');
	}
}


