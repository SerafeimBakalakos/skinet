import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IPagination } from '../shared/models/pagination';
import { IProduct } from '../shared/models/product';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = 'https://cocalhost:5001/api/';

  constructor(private http:HttpClient) { }

  getProducts() {
    return this.http.get<IPagination<IProduct[]>>(this.baseUrl + 'products');
  }
}
