import { Component } from '@angular/core';
import { MatExpansionModule } from '@angular/material/expansion';

@Component({
	selector: 'app-manip-in-run',
	standalone: true,
	templateUrl: './manip-in-run.component.html',
	styleUrls: ['./manip-in-run.component.css'],
	imports: [
		MatExpansionModule,
	],
})
export class ManipInRunComponent {
	manipDatetime: string;

	constructor() {
		let manipDt = localStorage.getItem('manipDatetime');
		if (manipDt == null) {
			this.manipDatetime = '[unknown]';
		} else {
			let date = new Date(manipDt);
			this.manipDatetime = `${date.toDateString()} ${date.toLocaleTimeString()}`;
		}
	}
}
