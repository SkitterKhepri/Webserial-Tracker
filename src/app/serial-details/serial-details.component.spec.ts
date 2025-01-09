import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SerialDetailsComponent } from './serial-details.component';

describe('SerialDetailsComponent', () => {
  let component: SerialDetailsComponent;
  let fixture: ComponentFixture<SerialDetailsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SerialDetailsComponent]
    });
    fixture = TestBed.createComponent(SerialDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
