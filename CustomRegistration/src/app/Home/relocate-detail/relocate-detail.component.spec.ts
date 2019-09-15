import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RelocateDetailComponent } from './relocate-detail.component';

describe('RelocateDetailComponent', () => {
  let component: RelocateDetailComponent;
  let fixture: ComponentFixture<RelocateDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RelocateDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RelocateDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
