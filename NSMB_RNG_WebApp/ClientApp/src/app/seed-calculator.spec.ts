import { TestBed } from '@angular/core/testing';

import { assert } from '../test/assert';
import { SeedCalculator } from './seed-calculator';

describe('SeedCalculator', () => {
	beforeEach(() => {
		TestBed.configureTestingModule({});
	});

	it('compute example seed', async () => {
		let seedCalc = new SeedCalculator('40:f4:07:f7:d4:21', new Date(2023, 11, 4, 1, 2, 3));
		seedCalc.timer0 = 400;
		seedCalc.vCount = 123;
		seedCalc.vFrame = 5;
		let seed = seedCalc.getSeed();
		assert(seedCalc.getSeed() == 0xf2b9aa20);
	});
});
