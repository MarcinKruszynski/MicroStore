import { Component, OnInit } from '@angular/core';
import { ConfigurationService } from '../configuration.service';
import { IProductItem } from '../productItem.model';
import { ProductService } from '../product.service';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})
export class ProductsComponent implements OnInit {

  products: IProductItem[];

  constructor(private productService: ProductService, private configurationService: ConfigurationService) { }  

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

  book(): void {
    
  }

}
