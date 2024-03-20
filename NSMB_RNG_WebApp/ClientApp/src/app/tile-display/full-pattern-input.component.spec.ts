import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FullPatternInputComponent } from './full-pattern-input.component';
import { MockHttpClient } from 'src/test/mockHttpClient';
import { HttpClient } from '@angular/common/http';
import { WorkerService } from '../worker.service';

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
		TestBed.overrideProvider(WorkerService, { useValue: {} }); // Don't create a worker thread; we don't use it.

		fixture = TestBed.createComponent(FullPatternInputComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});
});
