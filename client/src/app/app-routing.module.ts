import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ShopComponent } from './shop/shop.component';
import { ProductDetailsComponent } from './shop/product-details/product-details.component';

const routes: Routes = [
  {path:'', component:HomeComponent}, // homepage
  {path:'shop', component:ShopComponent},
  {path:'shop/:id', component:ProductDetailsComponent},
  {path:'**', redirectTo:'', pathMatch:'full'}, // send them to homepage
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
