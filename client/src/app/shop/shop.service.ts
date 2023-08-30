import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IPagination } from '../shared/models/pagination';
import { IProduct } from '../shared/models/product';
import { IBrand } from '../shared/models/brand';
import { IProductType } from '../shared/models/product-type';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = 'https://localhost:5001/api/';

  constructor(private http:HttpClient) { }

  getProducts(brandId?:number, typeId?:number, sort?:string) { //Order of parameters is important
    let prodParams = new HttpParams();
    
    if (brandId) prodParams = prodParams.append('brandId', brandId); 
    if (typeId) prodParams = prodParams.append('typeId', typeId); 
    if (sort) prodParams = prodParams.append('sort', sort); 

    return this.http.get<IPagination<IProduct[]>>(this.baseUrl + 'products', {params: prodParams});
  }

  getBrands() {
    return this.http.get<IBrand[]>(this.baseUrl + 'products/brands');
  }

  getProductTypes() {
    return this.http.get<IProductType[]>(this.baseUrl + 'products/types');
  }
}
