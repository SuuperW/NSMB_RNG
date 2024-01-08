import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Step6Component } from './step6.component';
import { RngParams } from '../../functions/rng-params-search';

describe('Step6Component', () => {
	let component: Step6Component;
	let fixture: ComponentFixture<Step6Component>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [ Step6Component ]
		})
			.compileComponents();

		// This component shouldn't ever attempt to load without certain items in localStorage.
		let rngParams: RngParams = {
			timer0: 1383,
			vCount: 39,
			vFrame: 5,
			mac: '40f407f7d421',
			is3DS: false,
			datetime: new Date('2023-11-23T07:00:15'),
			buttons: 0,
		};
		localStorage.setItem('rngParams', JSON.stringify(rngParams));
		localStorage.setItem('mac', rngParams.mac);
		localStorage.setItem('consoleType', rngParams.is3DS ? '3DS' : 'DS');

		fixture = TestBed.createComponent(Step6Component);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
