import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ShopComponent } from './shop.component';
import { ProductDetailsComponent } from './product-details/product-details.component';

const routes: Routes = [
  {path:'', component:ShopComponent},
  {path:':id', component:ProductDetailsComponent, data:{breadcrumb: {alias: 'productDetails'}}} 
  // The name of the product will be set to "productDetails" as alias of ":id" to use in breadcrumbs
]

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forChild(routes) //child routes of the root routing module (app-routing.module)
  ],
  exports: [
    RouterModule
  ]
})
export class ShopRoutingModule { }
