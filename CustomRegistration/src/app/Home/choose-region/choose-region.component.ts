import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CustomerService } from '../customer.service';

@Component({
  selector: 'app-choose-region',
  templateUrl: './choose-region.component.html',
  styleUrls: ['./choose-region.component.css']
})
export class ChooseRegionComponent implements OnInit {
url="/12";
  constructor(
    private router:Router,
    private customerService: CustomerService
    ) { }

  ngOnInit() {
  }
  onClickRegion(region){
    this.customerService.add({region});
    if(region=="geo"){

    this.router.navigate(['personalInfo'])}
    else{
      this.router.navigate(['personalInfoOther'])
    }
  }
 
}
