import { TestBed } from '@angular/core/testing';

import { SerialsService } from './serials.service';

describe('SerialsService', () => {
  let service: SerialsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SerialsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
