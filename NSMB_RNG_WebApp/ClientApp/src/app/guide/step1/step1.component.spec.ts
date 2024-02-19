import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Step1Component } from './step1.component';
import { GuideComponent } from '../guide.component';

describe('Step1Component', () => {
	let component: Step1Component;
	let fixture: ComponentFixture<Step1Component>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [Step1Component],
			providers: [{ provide: GuideComponent, useFactory: () => { return g; } }]
		})
			.compileComponents();
		let g = TestBed.createComponent(GuideComponent).componentInstance;

		fixture = TestBed.createComponent(Step1Component);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
