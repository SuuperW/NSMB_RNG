import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';

import { SeedTileCalculatorService } from './seeds-tile-calculator.service';
import { assert } from '../test/assert';

describe('SeedTileCalculatorService', () => {
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

	it('example bottom row', async () => {
		const seeds = [365998418, 2460521205];
		const bottom = ['ceebcpbiebe', 'ibbbbcebbib'];

		for (let i = 0; i < seeds.length; i++) {
			assert(service.getBottomRow(seeds[i]).toLowerCase() == bottom[i]);
		}
	});
});
