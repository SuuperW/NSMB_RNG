import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Step5Component } from './step5.component';
import { GuideComponent } from '../guide.component';

describe('Step5Component', () => {
	let component: Step5Component;
	let fixture: ComponentFixture<Step5Component>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [ Step5Component ],
			providers: [{ provide: GuideComponent, useFactory: () => { return g; } }]
		})
			.compileComponents();

		let g = TestBed.createComponent(GuideComponent).componentInstance;
		fixture = TestBed.createComponent(Step5Component);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
