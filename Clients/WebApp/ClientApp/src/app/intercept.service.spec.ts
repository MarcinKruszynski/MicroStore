import { TestBed } from '@angular/core/testing';

import { InterceptService } from './intercept.service';

describe('InterceptService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: InterceptService = TestBed.get(InterceptService);
    expect(service).toBeTruthy();
  });
});
