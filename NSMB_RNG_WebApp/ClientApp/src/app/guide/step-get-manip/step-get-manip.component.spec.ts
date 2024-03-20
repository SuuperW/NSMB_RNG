import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StepGetManipComponent } from './step-get-manip.component';
import { RngParams, SearchParams } from '../../functions/rng-params-search';
import { RouterTestingModule } from '@angular/router/testing';
import { GuideComponent } from '../guide.component';
import { MockHttpClient } from 'src/test/mockHttpClient';
import { HttpClient } from '@angular/common/http';
import { TimeFinderService } from 'src/app/time-finder.service';

describe('StepGetManipComponent', () => {
	let component: StepGetManipComponent;
	let fixture: ComponentFixture<StepGetManipComponent>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [ StepGetManipComponent, RouterTestingModule ],
			providers: [{ provide: GuideComponent, useFactory: () => { return g; } }]
		})
			.compileComponents();
		const mockHttpClient = new MockHttpClient();
		TestBed.overrideProvider(HttpClient, { useValue: mockHttpClient });
		const mockTimeFinder = { 
			getTime: (params: RngParams, route: string | number[]) => {
				return new Promise((r, e) => r(new Date('2000-01-01 05:12:15')));
			}
		}
		TestBed.overrideProvider(TimeFinderService, { useValue: mockTimeFinder });
	
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

		fixture = TestBed.createComponent(StepGetManipComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
