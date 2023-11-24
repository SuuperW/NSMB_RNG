import { AfterViewChecked, AfterViewInit, ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';

import { ComponentContainer } from '../component-container';
import { StepComponent } from './step';
import { Step1Component } from './step1/step1.component';
import { Step2Component } from './step2/step2.component';
import { Step3Component } from './step3/step3.component';
import { Step4Component } from './step4/step4.component';

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
	],
})
export class GuideComponent implements AfterViewInit {
	stepComponentList: TI[] = [
		Step1Component, Step2Component, Step3Component, Step4Component,
	];

	@ViewChild(ComponentContainer<StepComponent>) stepContainer!: ComponentContainer<StepComponent>;
	get stepComponent() {
		return this.stepContainer?.component;
	}

	currentStep: number = 0;

	get dumbInvalidGetter() {
		return (this.stepComponent?.form.invalid ?? true) ? '' : null;
	}

	constructor(private cdr: ChangeDetectorRef) {
		if (!localStorage.getItem('consoleType'))
			return;

		this.currentStep = 1;
		if (!localStorage.getItem('mac'))
			return;

		this.currentStep = 2;
		if (!localStorage.getItem('datetime'))
			return;

		this.currentStep = 3;
	}

	ngAfterViewInit() {
		// Do I need these?
		this.stepComponent?.form.markAsDirty();
		this.cdr.detectChanges();
	}

	next() {
		if (this.stepComponent?.form.invalid) {
			alert(this.getErrorsForForm(this.stepComponent?.form));
			return;
		}

		if (this.currentStep != this.stepComponentList.length - 1)
			this.currentStep++;
	}

	previous() {
		if (this.currentStep != 0)
			this.currentStep--;
	}

	getErrorsForForm(form: FormGroup) {
		let sb = [];
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


