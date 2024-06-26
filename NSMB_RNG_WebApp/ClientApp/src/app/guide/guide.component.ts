import { AfterViewInit, ChangeDetectorRef, Component, ViewChild, inject } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { ComponentContainer } from '../component-container';
import { StepComponent } from './step';
import { StepConsoleComponent } from './step-console/step-console.component';
import { StepMacComponent } from './step-mac/step-mac.component';
import { StepDateComponent } from './step-date/step-date.component';
import { StepTilesComponent } from './step-tiles/step-tiles.component';
import { StepRouteComponent } from './step-route/step-route.component';
import { StepGetManipComponent } from './step-get-manip/step-get-manip.component';
import { CommonModule } from '@angular/common';
import { Step0Component } from './step0/step0.component';
import { PopupDialogComponent, PopupDialogData } from '../popup-dialog/popup-dialog.component';
import { RngParams, SearchParams } from '../functions/rng-params-search';
import { StepTimeComponent } from './step-time/step-time.component';
import { RngParamsSearchResultManager } from './rng-params-search-result-manager';

@Component({
	selector: 'app-guide',
	standalone: true,
	templateUrl: './guide.component.html',
	styleUrls: ['./guide.component.css'],
	imports: [		
		StepConsoleComponent,
		ComponentContainer,
		CommonModule,
		MatDialogModule,
	],
})
export class GuideComponent implements AfterViewInit {
	stepComponentList: any[] = [
		Step0Component, StepRouteComponent, StepConsoleComponent, StepMacComponent,
		StepDateComponent, StepTimeComponent, StepTilesComponent,
		StepGetManipComponent,
	];

	dialog: MatDialog = inject(MatDialog);

	@ViewChild(ComponentContainer<StepComponent>) stepContainer!: ComponentContainer<StepComponent>;
	get stepComponent() {
		return this.stepContainer?.component;
	}

	currentStep: number = 0;

	isInProgress: boolean = false;
	progressStatus: string[] = [];

	paramsRange?: SearchParams;
	expectedParams?: RngParams;
	resultManager?: RngParamsSearchResultManager;

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
					message: [this.getErrorsForForm(this.stepComponent?.form, this.stepComponent?.errorStatus)],
				}
			});
			return;
		}

		if (this.currentStep != this.stepComponentList.length - 1)
			this.currentStep++;
	}

	previous() {
		if (this.currentStep >= this.stepComponentList.indexOf(StepTilesComponent)) {
			this.dialog.open<PopupDialogComponent, PopupDialogData>(PopupDialogComponent, {
				data: {
					message: ['Are you sure you want to go back? You will lose all previously entered tile patterns.'],
					buttons: ['Yes', 'No'],
					buttonHandler: (t: string) => {
						if (t === 'Yes')
							this.currentStep--;
					},
				}
			});
			return;
		}

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


