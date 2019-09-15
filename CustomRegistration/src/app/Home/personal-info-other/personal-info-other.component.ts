import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CustomerService } from '../customer.service';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { Country } from '../relocate-detail/Country';
import {AccordionModule} from 'primeng/accordion';     //accordion and accordion tab
import {MenuItem} from 'primeng/api';  
import {CountryService} from "../../country.service";


@Component({
  selector: 'app-personal-info-other',
  templateUrl: './personal-info-other.component.html',
  styleUrls: ['./personal-info-other.component.css']
})
export class PersonalInfoOtherComponent implements OnInit {
  region;
  form;
  buttonState = false;
  buttonName: any = 'Show';
  url = "/12";

  countryList: Country[] = [
    {id: 1, Country: 'Georgia', Name: 'Georgia'},
    {id: 2, Country: 'Russia', Name: 'Russia'},
    {id: 3, Country: 'USA', Name: 'Russia'},
    {id: 4, Country: 'UK', Name: 'UK'},

  ];

  constructor(
    private router: Router,
    private customerService: CustomerService,
    private fb: FormBuilder,
    private countryService: CountryService
  ) {
  }

  ngOnInit() {
    this.form = this.fb.group({
      id: ['', [Validators.required]],
      fullName: ['', [Validators.required]],
      birthDate: ['', [Validators.required]],
      birthLoc: ['', [Validators.required]],
      address: ['', [Validators.required]]

    });

    this.countryService.getCountries().subscribe(countries => {
      // this.countryList = countries;
    });

    this.region = this.customerService.getCustomerData().region;

    this.form.get('id').valueChanges.subscribe(id => {
      if (id.length === 9) {

        this.customerService.getGuestDataById(id).subscribe(personalInfo => {
          if (personalInfo != null) {
            this.form.get('fullName').setValue(personalInfo["Initials"]);
            this.form.get('birthDate').setValue(personalInfo["BirthDate"]);
          }

        });
      }
    });
  }

  onSubmit() {
    this.customerService.add({personalInfo: this.form.value});
    this.router.navigate(['/relocatePurpose']);
  }

  show() {
    this.buttonState = true;
  }

  hide() {
    this.buttonState = false;
  }
}
