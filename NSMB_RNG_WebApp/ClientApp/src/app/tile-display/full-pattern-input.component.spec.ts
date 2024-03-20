import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FullPatternInputComponent } from './full-pattern-input.component';
import { MockHttpClient } from 'src/test/mockHttpClient';
import { HttpClient } from '@angular/common/http';

describe('FullPatternInputComponent', () => {
	let component: FullPatternInputComponent;
	let fixture: ComponentFixture<FullPatternInputComponent>;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [FullPatternInputComponent]
		})
		.compileComponents();
		const mockHttpClient = new MockHttpClient();
		TestBed.overrideProvider(HttpClient, { useValue: mockHttpClient });

		fixture = TestBed.createComponent(FullPatternInputComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
