import { Component, OnInit } from '@angular/core';
import { IProduct } from '../shared/models/product';
import { ShopService } from './shop.service';
import { IProductType } from '../shared/models/product-type';
import { IBrand } from '../shared/models/brand';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent implements OnInit {
  products: IProduct[] = [];
  brands: IBrand[] = [];
  productTypes: IProductType[] = [];
  
  constructor(private shopService: ShopService) {}

  
  ngOnInit(): void {
    this.getProducts();
    this.getBrands();
    this.getProductTypes();
  }

  getProducts()
  {
    // GET returns an observable and we need to subscribe to it (otherwise the request will be skipped) using an observer object {next:..., error:..., complete:...}
    this.shopService.getProducts().subscribe({
      next: response => this.products = response.data, // what to do after the response is received
      error: err => console.log(err), // what to do in case of error
      complete: () => {
        console.log('request completed');
        console.log('extra statements'); // after "complete", we automatically unsubscribe
      }
    })
  }

  getBrands()
  {
    // GET returns an observable and we need to subscribe to it (otherwise the request will be skipped) using an observer object {next:..., error:..., complete:...}
    this.shopService.getBrands().subscribe({
      next: response => this.brands = response, // what to do after the response is received
      error: err => console.log(err) // what to do in case of error
    })
  }

  getProductTypes()
  {
    // GET returns an observable and we need to subscribe to it (otherwise the request will be skipped) using an observer object {next:..., error:..., complete:...}
    this.shopService.getProductTypes().subscribe({
      next: response => this.productTypes = response, // what to do after the response is received
      error: err => console.log(err) // what to do in case of error
    })
  }
}
