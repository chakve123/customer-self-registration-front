import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { CurrencyPipe } from '@angular/common';
import { CurrencyService } from 'src/app/currency.service';
import { CustomerService } from '../customer.service';

@Component({
  selector: 'app-info-about-goods',
  templateUrl: './info-about-goods.component.html',
  styleUrls: ['./info-about-goods.component.css']
})
export class InfoAboutGoodsComponent implements OnInit {
  form;
  info = [];
  rates = [{ usd: 1 }, { gel: 2 }]
  constructor(
    private fb: FormBuilder,
    private currencyService: CurrencyService,
    private customerService: CustomerService
  ) { }

  ngOnInit() {
    this.form = this.fb.group({
      paymentType: ['', [Validators.required]],
      quantity: ['', [Validators.required]],
      currency: ['', [Validators.required]],
      rate: ['', [Validators.required]],
    });

    this.form.get('currency').valueChanges.subscribe(val => {
      // this.currencyService.getRates().subscribe(rates => {
      //   this.rates = rates;
      // })
      this.form.get('rate').setValue(this.rates[1].gel);
    });
  }

  add() {
    if (this.form.status !== 'INVALID') {
      this.info.push(this.form.value);
      this.form.reset();
    }
  }

  remove(item) {
    const index = this.info.indexOf(item);
    this.info.splice(index, 1);
  }

  onClick() {
    this.customerService.add({ goods: this.info });
  }

}
