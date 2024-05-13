import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotifacationComponent } from './notifacation.component';

describe('NotifacationComponent', () => {
  let component: NotifacationComponent;
  let fixture: ComponentFixture<NotifacationComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [NotifacationComponent]
    });
    fixture = TestBed.createComponent(NotifacationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
