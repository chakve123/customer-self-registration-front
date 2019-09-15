import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RelocatePurposeComponent } from './relocate-purpose.component';

describe('RelocatePurposeComponent', () => {
  let component: RelocatePurposeComponent;
  let fixture: ComponentFixture<RelocatePurposeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RelocatePurposeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RelocatePurposeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
