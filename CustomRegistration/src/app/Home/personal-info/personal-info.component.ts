import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {CustomerService} from '../customer.service';
import {FormBuilder, FormControl, Validators} from '@angular/forms';

@Component({
  selector: 'app-personal-info',
  templateUrl: './personal-info.component.html',
  styleUrls: ['./personal-info.component.css']
})
export class PersonalInfoComponent implements OnInit {
  region;
  form;
  show: boolean = false;
  buttonName: any = 'Show';
  url = "/12";

  constructor(
    private router: Router,
    private customerService: CustomerService,
    private fb: FormBuilder
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

    this.region = this.customerService.getCustomerData().region;

    this.form.get('id').valueChanges.subscribe(id => {
      if (id.length === 11) {
        this.customerService.getCustomerDataById(id).subscribe(personalInfo => {

          this.form.get('fullName').setValue(personalInfo["Initials"]);
          this.form.get('birthDate').setValue(personalInfo["BirthDate"]);
          this.form.get('birthLoc').setValue(personalInfo["BirthLocation"]);
          this.form.get('address').setValue(personalInfo["Address"]);
        });
      }
    });
  }


  onSubmit() {
    this.customerService.add({personalInfo: this.form.value});
    this.router.navigate(['/relocatePurpose']);
  }
}
