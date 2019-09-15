import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CustomerService } from '../customer.service';
import { FormBuilder, FormControl } from '@angular/forms';
import { Country } from '../relocate-detail/Country';


@Component({
  selector: 'app-personal-info-other',
  templateUrl: './personal-info-other.component.html',
  styleUrls: ['./personal-info-other.component.css']
})
export class PersonalInfoOtherComponent implements OnInit {
  region;
  form;
  show: boolean = false;
  buttonName: any = 'Show';
  url = "/12";

  countryList:Country[]=[
    {id:1,Country:'Georgia',Name:'Georgia'},
    {id:2,Country:'Russia',Name:'Russia'},
    {id:3,Country:'USA',Name:'Russia'},
    {id:4,Country:'UK',Name:'UK'},

  ]

  constructor( private router: Router,
    private customerService: CustomerService,
    private fb: FormBuilder
    ) { }

  ngOnInit() {
    this.form = this.fb.group({
      id: [''],
      fullName: [''],
      birthDate: [''],
      birthLoc: [''],
      address: ['']

    });

    this.region = this.customerService.getCustomerData().region;

    this.form.get('id').valueChanges.subscribe(id => {
      if (id.length === 9) {
        
        this.customerService.getGuestDataById(id).subscribe(personalInfo => {
          console.log(personalInfo)
          if(personalInfo!=null){
          this.form.get('fullName').setValue(personalInfo["Initials"])
          this.form.get('birthDate').setValue(personalInfo["BirthDate"])       
          }

        });


        this.show = !this.show;



        this.buttonName = "Show";

      }
      else
        this.buttonName = "Hide";

    });
  }
  onBack() {
    this.router.navigate(['/chooseRegion' + this.url])
  }
  onSubmit() {
    this.router.navigate(['/relocatePurpose' + this.url])


  }

}
