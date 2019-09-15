import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonalInfoOtherComponent } from './personal-info-other.component';

describe('PersonalInfoOtherComponent', () => {
  let component: PersonalInfoOtherComponent;
  let fixture: ComponentFixture<PersonalInfoOtherComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PersonalInfoOtherComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PersonalInfoOtherComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
