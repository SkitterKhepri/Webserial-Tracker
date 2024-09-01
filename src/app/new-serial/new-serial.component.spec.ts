import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NewSerialComponent } from './new-serial.component';

describe('NewSerialComponent', () => {
  let component: NewSerialComponent;
  let fixture: ComponentFixture<NewSerialComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [NewSerialComponent]
    });
    fixture = TestBed.createComponent(NewSerialComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
