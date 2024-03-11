import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StepTimeComponent } from './step-time.component';
import { GuideComponent } from '../guide.component';

describe('StepTimeComponent', () => {
	let component: StepTimeComponent;
	let fixture: ComponentFixture<StepTimeComponent>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [StepTimeComponent],
			providers: [{ provide: GuideComponent, useFactory: () => { return g; } }]
		})
		.compileComponents();
		
		let g = TestBed.createComponent(GuideComponent).componentInstance;
		fixture = TestBed.createComponent(StepTimeComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
