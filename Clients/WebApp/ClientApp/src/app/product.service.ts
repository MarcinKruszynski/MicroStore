import { Injectable } from '@angular/core';
import { Response } from '@angular/http';

import { DataService } from './data.service';
import { ConfigurationService } from './configuration.service';
import { IProductItem } from './productItem.model';

import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  private productsUrl: string = '';  

  constructor(private service: DataService, private configurationService: ConfigurationService) {
    this.configurationService.settingsLoaded$.subscribe(x => {
        this.productsUrl = this.configurationService.serverSettings.gatewayApiUrl + '/api/v1/p/products';        
    });
  }

  getProducts(): Observable<IProductItem[]> {
    return this.service.get(this.productsUrl)
            .pipe(
                map((response: any) => {
                    return response;
                })
            );
  }

}
