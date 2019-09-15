import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InfoGoodsComponent } from './info-goods.component';

describe('InfoGoodsComponent', () => {
  let component: InfoGoodsComponent;
  let fixture: ComponentFixture<InfoGoodsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InfoGoodsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InfoGoodsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
