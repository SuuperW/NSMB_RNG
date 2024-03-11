import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StepRouteComponent } from './step-route.component';
import { GuideComponent } from '../guide.component';

describe('StepRouteComponent', () => {
	let component: StepRouteComponent;
	let fixture: ComponentFixture<StepRouteComponent>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [ StepRouteComponent ],
			providers: [{ provide: GuideComponent, useFactory: () => { return g; } }]
		})
			.compileComponents();

		let g = TestBed.createComponent(GuideComponent).componentInstance;
		fixture = TestBed.createComponent(StepRouteComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
