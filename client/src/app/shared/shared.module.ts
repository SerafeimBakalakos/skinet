import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PaginationModule } from 'ngx-bootstrap/pagination';


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    PaginationModule.forRoot() // loads this module as Singleton (we will use a single instance in many components)
  ],
  exports: [
    PaginationModule
  ]
})
export class SharedModule { }
