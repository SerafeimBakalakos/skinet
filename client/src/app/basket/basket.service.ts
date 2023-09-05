import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Basket, BasketTotals, IBasket, IBasketItem } from '../shared/models/basket';
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

  private basketTotalSource = new BehaviorSubject<BasketTotals | null>(null);
  basketTotalSource$ = this.basketTotalSource.asObservable();

  constructor(private http:HttpClient) { }

  // I would name this fetchBasket(). getBasket() makes sense in terms of HTTP get, but then setBasket should be named postBasket()
  getBasket(id:string) {
    return this.http.get<IBasket>(this.baseUrl + 'basket?id=' + id).subscribe({ 
      // next sets the field "basketSource" to the value of "basket" and notifies the subscribers
      next: basket => {
        this.basketSource.next(basket);
        this.calculateTotals(); 
      }
    });
  }

  setBasket(basket:IBasket) {
    return this.http.post<IBasket>(this.baseUrl + 'basket', basket).subscribe({
      // next sets the field "basketSource" to the value of "bask" (the one returned by the API, not the method param) and notifies the subscribers
      next: bask => {
        this.basketSource.next(bask);
        this.calculateTotals();
      }
    });
  }

  // In "getBasket" we update "basketSource" from the API and then notify the components. Here we just view and return a property of the client-side "basketSource". Naming both "get..." is very confusing.
  getCurrentBasketValue() {
    return this.basketSource.value;
  }

  addItemToBasket(item:IProduct | IBasketItem, quantity=1) {
    if (this.isProduct(item)) {
      item = this.mapProductItemToBasketItem(item);
    }
    
    // "getCurrentBasketValue()"" can return null, if there is no basket stored in the API
    const basket = this.getCurrentBasketValue() ?? this.createBasket();
    basket.items = this.addOrUpdateItem(basket.items, item, quantity);
    this.setBasket(basket);
  }

  removeItemFromBasket(id: number, quantity = 1) {
    const basket = this.getCurrentBasketValue();
    if (!basket) return;
    const item = basket.items.find(x => x.id === id);
    if (item) {
      item.quantity -= quantity;
      if (item.quantity <= 0) { // remove the item
        basket.items = basket.items.filter(x => x.id !== id);
      }
      if (basket.items.length > 0) {
        this.setBasket(basket);
      } else {
        this.deleteBasket(basket);
      }
    }
  }

  // Delete it from the service and local storage
  deleteBasket(basket: IBasket) {
    return this.http.delete(this.baseUrl + 'basket?id=' + basket.id).subscribe({
      next: () => {
        this.basketSource.next(null);
        this.basketTotalSource.next(null);
        localStorage.removeItem('basket_id');
      }
    });
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

  private calculateTotals()
  {
    const basket = this.getCurrentBasketValue();
    if (!basket) return;
    const shipping = 0;
    const subtotal = basket.items.reduce(
      (sum, prod) => sum + prod.price * prod.quantity, 0
    );
    const total = subtotal + shipping;
    this.basketTotalSource.next({shipping, total, subtotal});
  }

  private isProduct(item: IProduct | IBasketItem): item is IProduct {
    return (item as IProduct).productBrand !== undefined;
  }
}
 