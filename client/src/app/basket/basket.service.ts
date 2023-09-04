import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IBasket } from '../shared/models/basket';
import { HttpClient } from '@angular/common/http';

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
}
