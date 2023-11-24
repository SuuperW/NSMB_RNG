import { TestBed } from '@angular/core/testing';

import { SeedsByRow1Service } from './seeds-by-row1.service';

describe('SeedsByRow1Service', () => {
  let service: SeedsByRow1Service;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SeedsByRow1Service);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
