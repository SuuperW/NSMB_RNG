import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StepDateComponent } from './step-date.component';
import { GuideComponent } from '../guide.component';

describe('Step3Component', () => {
	let component: StepDateComponent;
	let fixture: ComponentFixture<StepDateComponent>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [StepDateComponent],
			providers: [{ provide: GuideComponent, useFactory: () => { return g; } }]
		})
			.compileComponents();

		let g = TestBed.createComponent(GuideComponent).componentInstance;
		fixture = TestBed.createComponent(StepDateComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
