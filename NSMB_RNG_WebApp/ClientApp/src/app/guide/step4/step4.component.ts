import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StepComponent } from '../step';
import { TileDisplayComponent } from '../../tile-display/tile-display.component';
import { SeedsByRow1Service } from '../../seeds-by-row1.service';

@Component({
	selector: 'app-step4',
	standalone: true,
	templateUrl: './step4.component.html',
	styleUrls: ['./step4.component.css'],
	imports: [
		ReactiveFormsModule,
		TileDisplayComponent,
	],
})
export class Step4Component implements StepComponent {
	seedService: SeedsByRow1Service = inject(SeedsByRow1Service);

	form = new FormGroup({
		row1Input: new FormControl(''),
		row2Input: new FormControl(''),
	});

	targetDateTime: string;

	submitCount: number = 0;

	inProgress: boolean = false;
	status: string = '';

	constructor() {
		let dtStr = localStorage.getItem('datetime');
		if (dtStr) {
			let dt = new Date(dtStr);
			this.targetDateTime = dt.toLocaleString();
		} else {
			this.targetDateTime = 'INVALID [go back and enter a date and time!]';
		}
	}

	submit() {
		this.inProgress = true;
		this.status = 'Nothing is actually happening.';
	}

	row1Changed(tiles: string) {
		if (!tiles || tiles.length != 7)
			return;

		let seeds = this.seedService.getPossibleSeedsFor(tiles);
	}
}
