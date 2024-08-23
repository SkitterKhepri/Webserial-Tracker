import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminSerialsComponent } from './admin-serials.component';

describe('AdminSerialsComponent', () => {
  let component: AdminSerialsComponent;
  let fixture: ComponentFixture<AdminSerialsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AdminSerialsComponent]
    });
    fixture = TestBed.createComponent(AdminSerialsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
