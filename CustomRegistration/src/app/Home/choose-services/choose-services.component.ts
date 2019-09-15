import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {CustomerService} from "../customer.service";

@Component({
  selector: 'app-choose-services',
  templateUrl: './choose-services.component.html',
  styleUrls: ['./choose-services.component.css']
})
export class ChooseServicesComponent implements OnInit {

  constructor(
    private router: Router,
    private customerService: CustomerService
    ) {
  }

  ngOnInit() {
  }

  onClick(service) {
    this.customerService.add({service})
    this.router.navigate(['/chooseRegion']);
  }

  Tax_Free() {
    alert("WORK");
  }
}
