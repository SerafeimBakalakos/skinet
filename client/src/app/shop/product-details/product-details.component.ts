import { Component, OnInit } from '@angular/core';
import { IProduct } from 'src/app/shared/models/product';
import { ShopService } from '../shop.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss']
})
export class ProductDetailsComponent implements OnInit {
  product?: IProduct;
  
  constructor(private shopService: ShopService, private activatedRoute:ActivatedRoute) {}

  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct() {
    // When clicking on a route, that route is activated and its parameters can accessed through ActivatedRoute.
    // The parameter we want was defined as "id" in the app-routing module.
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (id) {
      // Now that id is a non-null string, we cast it to number by writing "+id"
      this.shopService.getProduct(+id).subscribe({
        next: product => this.product = product,
        error: err => console.log(err)
      });
    }
    
  }
}
