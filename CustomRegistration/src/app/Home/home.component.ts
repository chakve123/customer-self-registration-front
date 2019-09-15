import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  countryList = [
    {name:'საქართველო', imgPath:'../assets/imgs/flag-geo.png'},
    {name:'ENGLISH', imgPath:'../assets/imgs/UK.png'},
    {name:'Russia', imgPath:'../assets/imgs/ru.png'},
    {name:'Espanol', imgPath:'../assets/imgs/es.png'},
    {name:'Turcia', imgPath:'../assets/imgs/tr.png'},
    {name:'Francia', imgPath:'../assets/imgs/fr.png'},
    {name:'Armenia', imgPath:'../assets/imgs/am.png'},
    {name:'Canada', imgPath:'../assets/imgs/ca.png'},
    {name:'China', imgPath:'../assets/imgs/cn.png'},
    {name:'Deutsch', imgPath:'../assets/imgs/de.png'},
    
    
    {name:'Japan', imgPath:'../assets/imgs/jp.png'},
    {name:'North Korea', imgPath:'../assets/imgs/kp.png'},
    {name:'portugalues', imgPath:'../assets/imgs/pt.png'},
    
    
    
    
  ]
  url="/12";
  constructor(private router:Router) { }

  ngOnInit() {
  }

  onClick(){
    this.router.navigate(['/chooseServices'+this.url]);
    
  }
  
}
