
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


const url=12;
const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'chooseServices/'+url, component: ChooseServicesComponent },
  { path: 'home/'+url, component: HomeComponent },
  { path: 'personalInfo/'+url, component: PersonalInfoComponent },
  { path: 'infoGoods/'+url, component: InfoGoodsComponent },
  { path: 'relocatePurpose/'+url, component:RelocatePurposeComponent },
  { path: 'successPage/'+url, component: SuccessPageComponent },
  { path: 'questions/'+url, component: QuestionsComponent },
  { path: 'InputInfoGoods/'+url, component: InputInfoGoodsComponent },
  { path: 'chooseRegion/'+url, component: ChooseRegionComponent },
  { path: 'relocateDetail/'+url, component: RelocateDetailComponent },
  { path: 'personalInfoOther/'+url, component: PersonalInfoOtherComponent },

 

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
