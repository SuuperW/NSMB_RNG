import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Step6Component } from './step6.component';
import { RngParams, SearchParams } from '../../functions/rng-params-search';
import { RouterTestingModule } from '@angular/router/testing';
import { GuideComponent } from '../guide.component';

describe('Step6Component', () => {
	let component: Step6Component;
	let fixture: ComponentFixture<Step6Component>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [ Step6Component, RouterTestingModule ],
			providers: [{ provide: GuideComponent, useFactory: () => { return g; } }]
		})
			.compileComponents();


		let g = TestBed.createComponent(GuideComponent).componentInstance;
		g.expectedParams = {
			timer0: 1383,
			vCount: 39,
			vFrame: 5,
			mac: '40f407f7d421',
			is3DS: false,
			datetime: new Date('2023-11-23T07:00:15'),
			buttons: 0,
		};
		g.paramsRange = new SearchParams({
			...g.expectedParams,
			// The actual values here do not matter for the existing tests.
			minTimer0: 0,
			maxTimer0: 0,
		})

		fixture = TestBed.createComponent(Step6Component);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
