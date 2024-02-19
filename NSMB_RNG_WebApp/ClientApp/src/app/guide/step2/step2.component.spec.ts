import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Step2Component } from './step2.component';
import { GuideComponent } from '../guide.component';

describe('Step2Component', () => {
	let component: Step2Component;
	let fixture: ComponentFixture<Step2Component>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [ Step2Component ],
			providers: [{ provide: GuideComponent, useFactory: () => { return g; } }]
		})
		.compileComponents();

		let g = TestBed.createComponent(GuideComponent).componentInstance;
		fixture = TestBed.createComponent(Step2Component);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
