import { Component } from '@angular/core';

@Component({
	selector: 'app-counter-component',
	standalone: true,
	templateUrl: './counter.component.html',
})
export class CounterComponent {
	public currentCount = 0;

	public incrementCounter() {
		this.currentCount++;
	}
}
