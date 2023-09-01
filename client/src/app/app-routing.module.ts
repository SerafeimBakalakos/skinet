import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { TestErrorComponent } from './core/test-error/test-error.component';
import { NotFoundComponent } from './core/not-found/not-found.component';
import { ServerErrorComponent } from './core/server-error/server-error.component';

const routes: Routes = [
  {path:'', component:HomeComponent, data:{breadcrumb:'Home'}}, // homepage
  {path:'test-error', component:TestErrorComponent}, // homepage
  {path:'not-found', component:NotFoundComponent}, // homepage
  {path:'server-error', component:ServerErrorComponent}, // homepage
  {path:'shop', loadChildren: () => import('./shop/shop.module').then(m => m.ShopModule)}, // lazy loading of this module's components
  {path:'**', redirectTo:'', pathMatch:'full'}, // send them to homepage
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
