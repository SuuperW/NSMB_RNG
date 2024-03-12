import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FullPatternInputComponent } from './full-pattern-input.component';

describe('FullPatternInputComponent', () => {
	let component: FullPatternInputComponent;
	let fixture: ComponentFixture<FullPatternInputComponent>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [FullPatternInputComponent]
		})
		.compileComponents();
		
		fixture = TestBed.createComponent(FullPatternInputComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
