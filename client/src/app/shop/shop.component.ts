import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { IProduct } from '../shared/models/product';
import { ShopService } from './shop.service';
import { IProductType } from '../shared/models/product-type';
import { IBrand } from '../shared/models/brand';
import { ShopParams } from '../shared/models/shop-params';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent implements OnInit {
  @ViewChild('search') searchTerm?: ElementRef; // Angular's ViewChild can access the HTML reference variable #search, which was defined in the input element
  products: IProduct[] = [];
  brands: IBrand[] = [];
  productTypes: IProductType[] = [];
  shopParams = new ShopParams();
  sortOptions = [
    {name:'Alphabetical', value:'name'},
    {name:'Price: low to high', value:'priceAsc'},
    {name:'Price: high to low', value:'priceDsc'}
  ]
  totalCount = 0;
  
  constructor(private shopService: ShopService) {}
  
  ngOnInit(): void {
    this.getProducts();
    this.getBrands();
    this.getProductTypes();
  }

  getProducts() {
    // GET returns an observable and we need to subscribe to it (otherwise the request will be skipped) using an observer object {next:..., error:..., complete:...}
    this.shopService.getProducts(this.shopParams).subscribe({
      next: response => {
        this.products = response.data 
        this.shopParams.pageIndex = response.pageIndex;
        this.shopParams.pageSize = response.pageSize;
        this.totalCount = response.count;
      }, // what to do after the response is received
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
    this.shopParams.brandId = brandId;
    this.shopParams.pageIndex = 1; // Fixes bug: filtering while being on page > 1 
    this.getProducts();
  }

  onTypeSelected(typeId: number) {
    this.shopParams.typeId = typeId;
    this.shopParams.pageIndex = 1;
    this.getProducts();
  }

  // The event object will be received by the API. It is very tricky to use a specific type, thus we forgo strong typing here
  onSortSelected(event: any) {
    this.shopParams.sort = event.target.value;
    this.getProducts();
  }

  onPageChanged(event: any) {
    if (this.shopParams.pageIndex !== event) { //event is the emitted page number
      this.shopParams.pageIndex = event;
      this.getProducts();
    }
  }

  onSearch() {
    this.shopParams.search = this.searchTerm?.nativeElement.value;
    this.shopParams.pageIndex = 1;
    this.getProducts();
  }

  onReset() {
    if (this.searchTerm) this.searchTerm.nativeElement.value = ''; 
    // why not: this.searchTerm?.nativeElement.value = '' 
    
    this.shopParams = new ShopParams();
    this.getProducts();
  }
}
