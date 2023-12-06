import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { SeedTileCalculatorService } from './seeds-tile-calculator.service';

describe('SeedsByRow1Service', () => {
	let service: SeedTileCalculatorService;

	beforeEach(() => {
		TestBed.configureTestingModule({
			imports: [HttpClientTestingModule],
		});
		service = TestBed.inject(SeedTileCalculatorService);
	});

	it('should be created', () => {
		expect(service).toBeTruthy();
	});
});
