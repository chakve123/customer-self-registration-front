import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InfoAboutGoodsComponent } from './info-about-goods.component';

describe('InfoAboutGoodsComponent', () => {
  let component: InfoAboutGoodsComponent;
  let fixture: ComponentFixture<InfoAboutGoodsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InfoAboutGoodsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InfoAboutGoodsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
