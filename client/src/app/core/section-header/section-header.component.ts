import { Component } from '@angular/core';
import { BreadcrumbService } from 'xng-breadcrumb';

@Component({
  selector: 'app-section-header',
  templateUrl: './section-header.component.html',
  styleUrls: ['./section-header.component.scss']
})
export class SectionHeaderComponent {

  //public allows us to use "bcService" inside the html file (section-header.component.html)
  constructor(public bcService:BreadcrumbService) {
    bcService.breadcrumbs$ //$ suffix is naming convention for observable
    //
  }

}
