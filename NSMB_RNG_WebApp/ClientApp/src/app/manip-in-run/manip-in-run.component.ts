import { Component } from '@angular/core';

@Component({
	selector: 'app-manip-in-run',
	standalone: true,
	templateUrl: './manip-in-run.component.html',
	styleUrls: ['./manip-in-run.component.css']
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
