import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StepMacComponent } from './step-mac.component';
import { GuideComponent } from '../guide.component';

describe('Step2Component', () => {
	let component: StepMacComponent;
	let fixture: ComponentFixture<StepMacComponent>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [ StepMacComponent ],
			providers: [{ provide: GuideComponent, useFactory: () => { return g; } }]
		})
		.compileComponents();

		let g = TestBed.createComponent(GuideComponent).componentInstance;
		fixture = TestBed.createComponent(StepMacComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
