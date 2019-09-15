import { Component, OnInit } from '@angular/core';
import { HttpClient } from 'selenium-webdriver/http';

import { Router } from '@angular/router';
import { typeWithParameters } from '@angular/compiler/src/render3/util';

@Component({
  selector: 'app-questions',
  templateUrl: './questions.component.html',
  styleUrls: ['./questions.component.css']
})
export class QuestionsComponent implements OnInit {
  question_array = [
    { question: "იარაღი ან ასაფეთქებელი მასალა ", redirect: true, redirectUrl: "url" },
    { question: "ნარკოტიკული ან ფსიქოტროპული ნივთიერება", redirect: false, redirectUrl: "url" },
    { question: "ანტიკვარული ან ხელოვნების ნიმუში ", redirect: false, redirectUrl: "url" },
    { question: "მომწამვლელი ნივთიერება ან მედიკამენტი    ", redirect: false, redirectUrl: "url" },
    { question: "მცენარე ან/და ცხოველი, მათი ნაწილები ან მათგან მიღებული პროდუქტი ", redirect: false, redirectUrl: "url" },
    { question: "მაღალი სიხშირის რადიოელექტრონული მოწყობილობა, კავშირგაბმულობის საშუალება    ", redirect: false, redirectUrl: "url" },
    { question: "რადიოაქტიური მასალა    ", redirect: false, redirectUrl: "url" },
    { question: "ეკონომიკური საქმიანობისთვის განკუთვნილი ან/და იმპორტის გადასახდელით დაუბეგრავ რაოდენობასა და ღირებულებაზე მეტი საქონელი ", redirect: false, redirectUrl: "url" },
    { question: "საქართველოს საბაჟო ტერიტორიაზე დროებით შემოტანის საბაჟო პროცედურაში (იმპორტის გადასახდელისაგან სრულად გათავისუფლებით) მოსაქცევი, ბარგით ან ხელბარგით გადაადგილებული საქონელი   ", redirect: false, redirectUrl: "url" },
    { question: "საქართველოში შემოვდივარ მუდმივად საცხოვრებლად და განზრახული მაქვს შემოვიტანო საქართველოს საგადასახადო კოდექსის 199-ე მუხლის" + "დ.ე" + "ქვეპუნქტით გათვალისწინებული საქონელი, რომელიც ჩემთან ერთად არ გადაადგილდება ", redirect: false, redirectUrl: "url" },
    { question: "საქართველოს საბაჟო ტერიტორიიდან გასატანი 15 000 ლარზე ნაკლები საბაჟო ღირებულების საქონელი  ", redirect: true, redirectUrl: "url" },
    { question: "ნაღდი ფული (ეროვნული ან/და უცხოური ვალუტა), ჩეკები ან/და სხვა ფასიანი ქაღალდები, რომელთა ჯამური ოდენობა აღემატება 30 000 ლარს, ან მის ეკვივალენტს სხვა ვალუტაში     ", redirect: true, redirectUrl: "url" },
  ]

  index = 0;
  length = this.question_array.length;
  url="/12";

  constructor(private router: Router) {

  }

  ngOnInit() {
  }

  onYes() {
    if (this.index == this.length-1) {
      this.router.navigate(['/InputInfoGoods'])
    }
    else {
      this.router.navigate(['/infoGoods'])
    }
  }

  onNo() {
    if (this.index < this.length - 1) {

      this.index++;

      console.log(this.index, this.length)

    } else {
      this.router.navigate(['/infoGoods'])
    }
  }

}
