import { Component, OnInit } from '@angular/core';
import { IProduct } from 'src/app/shared/models/product';
import { ShopService } from '../shop.service';
import { ActivatedRoute } from '@angular/router';
import { BreadcrumbService } from 'xng-breadcrumb';
import { BasketService } from 'src/app/basket/basket.service';
import { take } from 'rxjs';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss']
})
export class ProductDetailsComponent implements OnInit {
  product?: IProduct;
  quantity = 1;
  quantityInBasket = 0;
  
  constructor(private shopService: ShopService, 
    private activatedRoute:ActivatedRoute, 
    private bcService:BreadcrumbService,
    private basketService:BasketService) 
    {
      this.bcService.set('@productDetails', ' '); // empty string (needs a char to work) to show while loading 
    }

  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct() {
    // When clicking on a route, that route is activated and its parameters can accessed through ActivatedRoute.
    // The parameter we want was defined as "id" in the app-routing module.
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (id) {
      // Now that id is a non-null string, we cast it to number by writing "+id"
      this.shopService.getProduct(+id).subscribe({
        next: product => {
          this.product = product;
          this.bcService.set('@productDetails', product.name);

          // With "take(1)" we get the value from the observable and then immediately unsubscribe
          this.basketService.basketSource$.pipe(take(1)).subscribe({
            next: basket => {
              const item = basket?.items.find(x => x.id === +id) //cast id to number
              if (item) {
                this.quantity = item.quantity;
                this.quantityInBasket = item.quantity;
              }
            }
          })
        },
        error: err => console.log(err)
      });
    }
  }

  incrementQuantity() {
    this.quantity++;
  }

  decrementQuantity() {
    this.quantity--;
  }

  updateBasket() {
    if (this.product) {
        // Two branches for add and remove seem redundant. Why not have a set method (and corresponding branch)
        if (this.quantity > this.quantityInBasket) {
          const itemsToAdd = this.quantity - this.quantityInBasket;
          this.quantityInBasket += itemsToAdd;
          this.basketService.addItemToBasket(this.product, itemsToAdd);
        } else {
          const itemsToRemove = this.quantityInBasket - this.quantity;
          this.quantityInBasket -= itemsToRemove;
          this.basketService.removeItemFromBasket(this.product.id, itemsToRemove);
        }
    }
  }

  // Getter property. Setters exist too and can be used as [Input]
  get buttonText() {
    return this.quantityInBasket === 0 ? 'Add to basket' : "Update basket";
  }
}
