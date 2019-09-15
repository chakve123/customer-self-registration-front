import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {CustomerService} from '../customer.service';

import {Country} from './Country';
import {CountryService} from "../../country.service";
import {FormBuilder, Validators} from "@angular/forms";
import {log} from "util";

@Component({
  selector: 'app-relocate-detail',
  templateUrl: './relocate-detail.component.html',
  styleUrls: ['./relocate-detail.component.css']
})
export class RelocateDetailComponent implements OnInit {
  url = "/12";
  form;


  countryList: Country[] = [
    {id: 1, Country: 'Georgia', Name: 'Georgia'},
    {id: 2, Country: 'Russia', Name: 'Russia'},
    {id: 3, Country: 'USA', Name: 'Russia'},
    {id: 4, Country: 'UK', Name: 'UK'},

  ];

  constructor(
    private router: Router,
    private countryService: CountryService,
    private fb: FormBuilder,
    private customerService: CustomerService
  ) {
  }

  ngOnInit() {
    this.form = this.fb.group({
      countryFrom: ['', [Validators.required]],
      carNumber: ['']
    });

    this.countryService.getCountries().subscribe(countries => {
      // this.countryList = countries;
    });
  }

  onSubmit() {
    this.customerService.add({relocateDetail: this.form.value});
    this.router.navigate(['/questions']);
  }

}
