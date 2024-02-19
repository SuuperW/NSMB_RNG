import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Step3Component } from './step3.component';
import { GuideComponent } from '../guide.component';

describe('Step3Component', () => {
	let component: Step3Component;
	let fixture: ComponentFixture<Step3Component>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [Step3Component],
			providers: [{ provide: GuideComponent, useFactory: () => { return g; } }]
		})
			.compileComponents();

		let g = TestBed.createComponent(GuideComponent).componentInstance;
		fixture = TestBed.createComponent(Step3Component);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
