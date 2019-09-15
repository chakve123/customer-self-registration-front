import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CustomerService } from '../customer.service';

import { Country } from './Country';

@Component({
  selector: 'app-relocate-detail',
  templateUrl: './relocate-detail.component.html',
  styleUrls: ['./relocate-detail.component.css']
})
export class RelocateDetailComponent implements OnInit {
  url="/12";


  countryList:Country[]=[
    {id:1,Country:'Georgia',Name:'Georgia'},
    {id:2,Country:'Russia',Name:'Russia'},
    {id:3,Country:'USA',Name:'Russia'},
    {id:4,Country:'UK',Name:'UK'},

  ]
  constructor(private router:Router) { }

  ngOnInit() {
    
  }
onSubmit(){

  this.router.navigate(['/questions'])


}

}
