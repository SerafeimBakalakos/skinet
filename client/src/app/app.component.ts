import { Component, OnInit } from '@angular/core';
import { BasketService } from './basket/basket.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

// OnInit is a lifecycle hook
export class AppComponent implements OnInit {
  title = 'Skinet';

  // Services can injected into the ctor of AppComponent
  // However it is too early to make an API request here
  constructor(private basketService: BasketService) {}
  
  ngOnInit(): void {
    const basketId = localStorage.getItem('basket_id'); //Shouldn't these local storage keys be stored in a central file?
    if (basketId) this.basketService.getBasket(basketId); // This will cause events to populate through other subscribers of BasketService.getBasket(...);
  }
}
