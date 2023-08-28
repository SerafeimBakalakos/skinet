import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { IProduct } from './models/product';
import { IPagination } from './models/pagination';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

// OnInit is a lifecycle hook
export class AppComponent implements OnInit {
  title = 'Skinet';
  products: IProduct[] = []; // initial value is necessary when using strict mode in TS

  // Services can injected into the ctor of AppComponent
  // However it is too early to make an API request here
  constructor(private http: HttpClient) {}
  
  ngOnInit(): void {
    // Best practice: make request to the API here.
    // GET returns an observable and we need to subscribe to it (otherwise the request will be skipped) using an observer object {next:..., error:..., complete:...}
    this.http.get<IPagination<IProduct[]>>('https://localhost:5001/api/products?pageSize=50').subscribe({
      next: response => this.products = response.data, // what to do after the response is received
      error: error => console.log(error), // what to do in case of error
      complete: () => {
        console.log('request completed');
        console.log('extra statements');
      }
    }) // after "complete", we automatically unsubscribe
  }
}
