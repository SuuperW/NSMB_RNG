import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SeedParamsFinderComponent } from './seed-params-finder.component';

describe('SeedParamsFinderComponent', () => {
  let component: SeedParamsFinderComponent;
  let fixture: ComponentFixture<SeedParamsFinderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SeedParamsFinderComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SeedParamsFinderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
