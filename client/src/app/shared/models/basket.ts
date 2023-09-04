import * as cuid from 'cuid';

export interface IBasketItem {
    id: number;
    productName: string;
    price: number;
    quantity: number;
    prictureUrl: string;
    brand: string;
    productType: string;
}

export interface IBasket {
    id: string;
    items: IBasketItem[];
}

export class Basket implements IBasket {
    id = cuid(); // Generate a unique id for each user
    items: IBasketItem[] = [];
}