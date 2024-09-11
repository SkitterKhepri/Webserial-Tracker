import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminNewserialComponent } from './admin-newserial.component';

describe('AdminNewserialComponent', () => {
  let component: AdminNewserialComponent;
  let fixture: ComponentFixture<AdminNewserialComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AdminNewserialComponent]
    });
    fixture = TestBed.createComponent(AdminNewserialComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
