import { Component, OnInit } from '@angular/core';
import { IProduct } from '../shared/models/product';
import { ShopService } from './shop.service';

@Component({
  selector: 'app-shop',
  templateUrl: './shop.component.html',
  styleUrls: ['./shop.component.scss']
})
export class ShopComponent implements OnInit {
  products: IProduct[] = [];
  
  constructor(private shopService: ShopService) {}

  // GET returns an observable and we need to subscribe to it (otherwise the request will be skipped) using an observer object {next:..., error:..., complete:...}
  ngOnInit(): void {
    this.shopService.getProducts().subscribe({
      next: response => this.products = response.data, // what to do after the response is received
      error: err => console.log(err), // what to do in case of error
      complete: () => {
        console.log('request completed');
        console.log('extra statements');
      } // after "complete", we automatically unsubscribe
    })
  }

}
