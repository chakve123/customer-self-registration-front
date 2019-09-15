import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChooseRegionComponent } from './choose-region.component';

describe('ChooseRegionComponent', () => {
  let component: ChooseRegionComponent;
  let fixture: ComponentFixture<ChooseRegionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChooseRegionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChooseRegionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
