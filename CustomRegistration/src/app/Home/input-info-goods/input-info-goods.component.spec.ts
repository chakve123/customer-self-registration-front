import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InputInfoGoodsComponent } from './input-info-goods.component';

describe('InputInfoGoodsComponent', () => {
  let component: InputInfoGoodsComponent;
  let fixture: ComponentFixture<InputInfoGoodsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InputInfoGoodsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InputInfoGoodsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
