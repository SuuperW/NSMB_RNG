import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StepConsoleComponent } from './step-console.component';
import { GuideComponent } from '../guide.component';

describe('Step1Component', () => {
	let component: StepConsoleComponent;
	let fixture: ComponentFixture<StepConsoleComponent>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [StepConsoleComponent],
			providers: [{ provide: GuideComponent, useFactory: () => { return g; } }]
		})
			.compileComponents();
		let g = TestBed.createComponent(GuideComponent).componentInstance;

		fixture = TestBed.createComponent(StepConsoleComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
