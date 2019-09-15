import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CustomerService } from '../customer.service';

@Component({
  selector: 'app-relocate-purpose',
  templateUrl: './relocate-purpose.component.html',
  styleUrls: ['./relocate-purpose.component.css']
})
export class RelocatePurposeComponent implements OnInit {
  action:string;
  url="/12";
  constructor(
    private router: Router,
    private customerService: CustomerService
    ) { }

  ngOnInit() {
  }

  onClick(purpose) {
    this.customerService.add({purpose});
    this.router.navigate(['/relocateDetail'+this.url])
  }

  onBack(){
    this.router.navigate(['/personalInfo'+this.url])
  }
}
