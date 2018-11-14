import { Component, OnInit } from '@angular/core';
import { ConfigurationService } from '../configuration.service';
import { IProductItem } from '../productItem.model';
import { ProductService } from '../product.service';
import { BookingService } from '../booking.service';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})
export class ProductsComponent implements OnInit {

  products: IProductItem[];

  constructor(private productService: ProductService, private bookingService: BookingService, private configurationService: ConfigurationService) { }  

  ngOnInit() {
    if (this.configurationService.isReady) {
      this.getProducts();
    } else {
        this.configurationService.settingsLoaded$.subscribe(x => {
            this.getProducts();
        });
    }   
  }  

  getProducts(): void {
    this.productService.getProducts()
      .subscribe(products => this.products = products);    
  }

  book(product: IProductItem): void {
    this.bookingService.book(product);
  }

}
