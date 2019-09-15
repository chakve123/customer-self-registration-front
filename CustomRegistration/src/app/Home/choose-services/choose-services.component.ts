import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-choose-services',
  templateUrl: './choose-services.component.html',
  styleUrls: ['./choose-services.component.css']
})
export class ChooseServicesComponent implements OnInit {
  
  constructor(private router:Router) { }

  ngOnInit() {
  }
 
  onPhisic() {
    this.router.navigate(['/chooseRegion'])
  }
  Tax_Free() {
    alert("WORK")
  }
}
