import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IPagination } from '../shared/models/pagination';
import { IProduct } from '../shared/models/product';
import { IBrand } from '../shared/models/brand';
import { IProductType } from '../shared/models/product-type';
import { ShopParams } from '../shared/models/shop-params';

@Injectable({
  providedIn: 'root'
})
export class ShopService {
  baseUrl = 'https://localhost:5001/api/';

  constructor(private http:HttpClient) { }

  getProducts(shopParams: ShopParams) { //Order of parameters is important
    let prodParams = new HttpParams();
    
    if (shopParams.brandId > 0) prodParams = prodParams.append('brandId', shopParams.brandId); 
    if (shopParams.typeId > 0) prodParams = prodParams.append('typeId', shopParams.typeId); 
    prodParams = prodParams.append('sort', shopParams.sort); 
    prodParams = prodParams.append('pageIndex', shopParams.pageIndex); 
    prodParams = prodParams.append('pageSize', shopParams.pageSize); 
    if (shopParams.search) prodParams = prodParams.append('search', shopParams.search);

    return this.http.get<IPagination<IProduct[]>>(this.baseUrl + 'products', {params: prodParams});
  }

  getBrands() {
    return this.http.get<IBrand[]>(this.baseUrl + 'products/brands');
  }

  getProductTypes() {
    return this.http.get<IProductType[]>(this.baseUrl + 'products/types');
  }
}
