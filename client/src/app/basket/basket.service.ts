import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Basket, IBasket, IBasketItem } from '../shared/models/basket';
import { HttpClient } from '@angular/common/http';
import { IProduct } from '../shared/models/product';

@Injectable({
  providedIn: 'root'
})
export class BasketService {
  baseUrl = environment.apiUrl;

  // This is an observable that our components will subscribe to.
  // BasketService is registered as a Singleton, therefore tother components will always have acces to the observable.
  // BehaviorSubject needs an initial value
  private basketSource = new BehaviorSubject<IBasket | null>(null);

  // The other components will actually subscribe to this one.
  basketSource$ = this.basketSource.asObservable();

  constructor(private http:HttpClient) { }

  // I would name this fetchBasket(). getBasket() makes sense in terms of HTTP get, but then setBasket should be named postBasket()
  getBasket(id:string) {
    return this.http.get<IBasket>(this.baseUrl + 'basket?id=' + id).subscribe({ 
      // next sets the field "basketSource" to the value of "basket" and notifies the subscribers
      next: basket => this.basketSource.next(basket) 
    });
  }

  setBasket(basket:IBasket) {
    return this.http.post<IBasket>(this.baseUrl + 'basket', basket).subscribe({
      // next sets the field "basketSource" to the value of "bask" (the one returned by the API, not the method param) and notifies the subscribers
      next: bask => this.basketSource.next(bask)
    });
  }

  // In "getBasket" we update "basketSource" from the API and then notify the components. Here we just view and return a property of the client-side "basketSource". Naming both "get..." is very confusing.
  getCurrentBasketValue() {
    return this.basketSource.value;
  }

  addItemToBasket(item:IProduct, quantity=1) {
    const itemToAdd = this.mapProductItemToBasketItem(item);
    // "getCurrentBasketValue()"" can return null, if there is no basket stored in the API
    const basket = this.getCurrentBasketValue() ?? this.createBasket();
    basket.items = this.addOrUpdateItem(basket.items, itemToAdd, quantity);
    this.setBasket(basket);
  }

  private addOrUpdateItem(items: IBasketItem[], itemToAdd: IBasketItem, quantity: number): IBasketItem[] {
    const item = items.find(x => x.id === itemToAdd.id);
    if (item) item.quantity += quantity;
    else {
      itemToAdd.quantity = quantity;
      items.push(itemToAdd);
    }
    return items;
  }

  private createBasket(): IBasket {
    const basket = new Basket();

    // persist the id in browser's local storage. It will persist even after computer reboots
    localStorage.setItem('basket_id', basket.id); 
    return basket;
  }

  private mapProductItemToBasketItem(product:IProduct): IBasketItem {
    return {
      id: product.id,
      productName: product.name,
      price: product.price,
      quantity: 0,
      pictureUrl: product.pictureUrl,
      brand: product.productBrand,
      productType: product.productType
    }
  }
}
