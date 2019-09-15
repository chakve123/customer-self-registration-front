import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-info-goods',
  templateUrl: './info-goods.component.html',
  styleUrls: ['./info-goods.component.css']
})
export class InfoGoodsComponent implements OnInit {
  url="/12";
  constructor(private router:Router) { }

  ngOnInit() {
  }
  goHome(){
    this.router.navigate([''])  }

}
