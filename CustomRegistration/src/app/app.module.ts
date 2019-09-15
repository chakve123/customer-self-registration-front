import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {ReactiveFormsModule, FormsModule} from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './Home/home.component';
import { ChooseServicesComponent } from './Home/choose-services/choose-services.component';
import { PersonalInfoComponent } from './Home/personal-info/personal-info.component';
import { RelocatePurposeComponent } from './Home/relocate-purpose/relocate-purpose.component';
import { InfoGoodsComponent } from './Home/info-goods/info-goods.component';
import { SuccessPageComponent } from './Home/success-page/success-page.component';
import { QuestionsComponent } from './Home/questions/questions.component';
import { InputInfoGoodsComponent } from './Home/input-info-goods/input-info-goods.component';
import { HeaderComponent } from './home/header/header.component';
import { ChooseRegionComponent } from './Home/choose-region/choose-region.component';
import { RelocateDetailComponent } from './Home/relocate-detail/relocate-detail.component';
import { HttpClientModule } from '@angular/common/http';
import { PersonalInfoOtherComponent } from './Home/personal-info-other/personal-info-other.component';
import {CalendarModule} from 'primeng/calendar';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HeaderNotificationComponent } from './Home/header-notification/header-notification.component';
import { InfoAboutGoodsComponent } from './Home/info-about-goods/info-about-goods.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ChooseServicesComponent,
    PersonalInfoComponent,
    RelocatePurposeComponent,
    InfoGoodsComponent,
    SuccessPageComponent,
    QuestionsComponent,
    InputInfoGoodsComponent,
    HeaderComponent,
    ChooseRegionComponent,
    RelocateDetailComponent,
    PersonalInfoOtherComponent,
    HeaderNotificationComponent,
    InfoAboutGoodsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule,
    CalendarModule,
    BrowserAnimationsModule

  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
