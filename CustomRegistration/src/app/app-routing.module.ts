
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { HomeComponent } from './Home/home.component';
import { ChooseServicesComponent } from './Home/choose-services/choose-services.component';
import { PersonalInfoComponent } from './Home/personal-info/personal-info.component';
import { InfoGoodsComponent } from './Home/info-goods/info-goods.component';
import { RelocatePurposeComponent } from './Home/relocate-purpose/relocate-purpose.component';
import { SuccessPageComponent } from './Home/success-page/success-page.component';
import { QuestionsComponent } from './Home/questions/questions.component';
import { InputInfoGoodsComponent } from './Home/input-info-goods/input-info-goods.component';
import { ChooseRegionComponent } from './Home/choose-region/choose-region.component';
import { RelocateDetailComponent } from './Home/relocate-detail/relocate-detail.component';
import { PersonalInfoOtherComponent } from './Home/personal-info-other/personal-info-other.component';
import { InfoAboutGoodsComponent } from './Home/info-about-goods/info-about-goods.component';



const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'chooseServices', component: ChooseServicesComponent },
  { path: 'home', component: HomeComponent },
  { path: 'personalInfo', component: PersonalInfoComponent },
  { path: 'infoGoods', component: InfoGoodsComponent },
  { path: 'relocatePurpose', component:RelocatePurposeComponent },
  { path: 'successPage', component: SuccessPageComponent },
  { path: 'questions', component: QuestionsComponent },
  { path: 'InputInfoGoods', component: InputInfoGoodsComponent },
  { path: 'chooseRegion', component: ChooseRegionComponent },
  { path: 'relocateDetail', component: RelocateDetailComponent },
  { path: 'personalInfoOther', component: PersonalInfoOtherComponent },
  { path: 'info', component: InfoAboutGoodsComponent },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
