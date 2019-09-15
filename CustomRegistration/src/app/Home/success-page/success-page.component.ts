import { Component, OnInit } from '@angular/core';
import { ThrowStmt } from '@angular/compiler';
import { Router } from '@angular/router';

@Component({
  selector: 'app-success-page',
  templateUrl: './success-page.component.html',
  styleUrls: ['./success-page.component.css']
})
export class SuccessPageComponent implements OnInit {
  FirstCode :string="0001";
  secondCode:number=11111;
  Year:number=2019;
  url="/12";
  constructor(private router:Router) { }

  ngOnInit() {
  }

  goHome(){

this.secondCode++;
this.router.navigate(['/home'])
  }
}
