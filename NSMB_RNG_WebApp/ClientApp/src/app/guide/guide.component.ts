import { AfterViewInit, ChangeDetectorRef, Component, ViewChild, inject } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { ComponentContainer } from '../component-container';
import { StepComponent } from './step';
import { Step1Component } from './step1/step1.component';
import { Step2Component } from './step2/step2.component';
import { Step3Component } from './step3/step3.component';
import { Step4Component } from './step4/step4.component';
import { Step5Component } from './step5/step5.component';
import { Step6Component } from './step6/step6.component';
import { Step7Component } from './step7/step7.component';
import { CommonModule } from '@angular/common';

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
	],
})
export class GuideComponent implements AfterViewInit {
	stepComponentList: TI[] = [
		Step1Component, Step2Component, Step3Component, Step4Component,
		Step5Component, Step6Component, Step7Component,
	];

	@ViewChild(ComponentContainer<StepComponent>) stepContainer!: ComponentContainer<StepComponent>;
	get stepComponent() {
		return this.stepContainer?.component;
	}

	currentStep: number = 0;

	isInProgress: boolean = false;
	progressStatus: string[] = [];

	constructor(private cdr: ChangeDetectorRef) {
		if (!localStorage.getItem('consoleType') || !localStorage.getItem('gameVersion'))
			return;

		this.currentStep = 1;
		if (!localStorage.getItem('mac'))
			return;

		this.currentStep = 2;
		if (!localStorage.getItem('datetime'))
			return;

		this.currentStep = 3;
	}

	onLoadStepComponent(component: StepComponent) {
		component.progress.add((progressStatus: string[]) => {
			this.isInProgress = progressStatus.length != 0;
			this.progressStatus = progressStatus.slice();
			// Since we're updating proerties outside of events that Angular expects to modify this component, we need to manually call detectChanges.
			this.cdr.detectChanges();
		});
		
	}

	ngAfterViewInit() {
		this.stepContainer.componentCreated.add(this.onLoadStepComponent);
		if (this.stepContainer.component)
			this.onLoadStepComponent(this.stepContainer.component);
	}

	next() {
		if (this.stepComponent?.form.invalid || this.stepComponent?.errorStatus) {
			alert(this.getErrorsForForm(this.stepComponent?.form, this.stepComponent?.errorStatus));
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


