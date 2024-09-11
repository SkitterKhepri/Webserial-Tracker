import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LikedSerialsComponent } from './liked-serials.component';

describe('LikedSerialsComponent', () => {
  let component: LikedSerialsComponent;
  let fixture: ComponentFixture<LikedSerialsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [LikedSerialsComponent]
    });
    fixture = TestBed.createComponent(LikedSerialsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
