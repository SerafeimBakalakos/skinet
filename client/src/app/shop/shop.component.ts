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
  brandIdSelected = 0;
  typeIdSelected = 0;
  sortSelected = 'name';
  sortOptions = [
    {name:'Alphabetical', value:'name'},
    {name:'Price: low to high', value:'priceAsc'},
    {name:'Price: high to low', value:'priceDsc'}
  ]
  
  constructor(private shopService: ShopService) {}
  
  ngOnInit(): void {
    this.getProducts();
    this.getBrands();
    this.getProductTypes();
  }

  getProducts() {
    // GET returns an observable and we need to subscribe to it (otherwise the request will be skipped) using an observer object {next:..., error:..., complete:...}
    this.shopService.getProducts(this.brandIdSelected, this.typeIdSelected, this.sortSelected).subscribe({
      next: response => this.products = response.data, // what to do after the response is received
      error: err => console.log(err), // what to do in case of error
      complete: () => {
        console.log('request completed');
        console.log('extra statements'); // after "complete", we automatically unsubscribe
      }
    })
  }

  getBrands() {
    this.shopService.getBrands().subscribe({
      next: response => this.brands = [{id:0, name: 'All'}, ...response], // spread operator will create an array with "All", response[1], response[2], etc.
      error: err => console.log(err) 
    })
  }

  getProductTypes() {
    this.shopService.getProductTypes().subscribe({
      next: response => this.productTypes = [{id:0, name: 'All'}, ...response],
      error: err => console.log(err)
    })
  }

  onBrandSelected(brandId: number) {
    this.brandIdSelected = brandId;
    this.getProducts();
  }

  onTypeSelected(typeId: number) {
    this.typeIdSelected = typeId;
    this.getProducts();
  }

  // The event object will be received by the API. It is very tricky to use a specific type, thus we forgo strong typing here
  onSortSelected(event: any) {
    this.sortSelected = event.target.value;
    this.getProducts();
  }
}
