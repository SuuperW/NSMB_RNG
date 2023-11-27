import { TestBed } from '@angular/core/testing';

import { SeedTileCalculatorService } from './seeds-tile-calculator.service';

describe('SeedsByRow1Service', () => {
  let service: SeedTileCalculatorService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SeedTileCalculatorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
