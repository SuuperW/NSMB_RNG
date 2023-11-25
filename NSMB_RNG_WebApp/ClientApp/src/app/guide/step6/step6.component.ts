import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { StepComponent } from '../step';
import { TileDisplayComponent } from '../../tile-display/tile-display.component';

@Component({
	selector: 'app-step6',
	standalone: true,
	templateUrl: './step6.component.html',
	styleUrls: ['./step6.component.css'],
	imports: [
		ReactiveFormsModule,
		TileDisplayComponent,
	],
})
export class Step6Component implements StepComponent {
	manipDatetime = localStorage.getItem('manipDatetime') ?? '[INVALID] Go back!';

	form = new FormGroup({
		row1Input: new FormControl(''),
	});
	errorStatus?= 'You haven\'t found the right tile pattern yet.';

	inProgress: boolean = false;
	status: string = '';
	feedback: string = '';

	async row1Changed(tiles: string) {
		if (!tiles || tiles.length != 7)
			return;

		this.inProgress = true;
		this.status = 'Checking tile pattern...';

		// TODO
		this.feedback = 'Tile pattern is good! Go to the next page.';
		this.errorStatus = undefined;

		this.inProgress = false;
	}
}
