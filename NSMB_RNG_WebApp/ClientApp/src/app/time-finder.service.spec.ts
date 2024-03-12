import { TestBed } from '@angular/core/testing';

import { TimeFinderService } from './time-finder.service';

describe('TimeFinderService', () => {
  let service: TimeFinderService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TimeFinderService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
