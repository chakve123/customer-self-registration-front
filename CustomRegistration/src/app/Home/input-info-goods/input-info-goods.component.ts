import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl } from '@angular/forms';
import { Route } from '@angular/compiler/src/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-input-info-goods',
  templateUrl: './input-info-goods.component.html',
  styleUrls: ['./input-info-goods.component.css']
})
export class InputInfoGoodsComponent implements OnInit {
 form;
 index=0;
 url="/12";
  constructor(private fb: FormBuilder,private router:Router) { }

  ngOnInit() {
    this.form=this.fb.group({
      cash: [''],
      number:[''],
      currency:[''],
      course:['']
    });


    (this.form.get("currency") as FormControl).valueChanges.subscribe(val =>{
      this.form.get("course").setValue(val);
    })
  }
  onBack(){

this.router.navigate(['/questions'+this.url])
  }
  confirm(){
    this.router.navigate(['successPage'+this.url])
  }
}
