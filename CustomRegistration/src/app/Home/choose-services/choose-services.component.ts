import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-choose-services',
  templateUrl: './choose-services.component.html',
  styleUrls: ['./choose-services.component.css']
})
export class ChooseServicesComponent implements OnInit {
  url="/12";
  constructor(private router:Router) { }

  ngOnInit() {
  }
  onBack(){
    this.router.navigate(['/home'+this.url])
  }
  onPhisic() {
    this.router.navigate(['/chooseRegion'+this.url])
  }
  Tax_Free() {
    alert("WORK")
  }
}
