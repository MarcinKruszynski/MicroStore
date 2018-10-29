import { Component, OnInit } from '@angular/core';
import { Product } from '../product';
import { ProductService } from '../product.service';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.scss']
})
export class ProductsComponent implements OnInit {

  products: Product[];

  constructor(private productService: ProductService) { }

  getProducts(): void {
    this.productService.getProducts()
      .subscribe(products => this.products = products);    
  }

  ngOnInit() {
    this.getProducts();
  }

  book(): void {
    
  }

}
