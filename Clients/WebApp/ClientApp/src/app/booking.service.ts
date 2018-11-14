import { Injectable } from '@angular/core';

import { DataService } from './data.service';
import { ConfigurationService } from './configuration.service';

import { Observable } from 'rxjs';

import { Guid } from '../guid';
import { map } from 'rxjs/operators';
import { IProductItem } from './productItem.model';

@Injectable({
  providedIn: 'root'
})
export class BookingService {

  private bookingUrl: string = '';  

  constructor(private service: DataService, private configurationService: ConfigurationService) {
    if (this.configurationService.isReady)
        this.bookingUrl = this.configurationService.serverSettings.gatewayApiUrl + '/api/v1/b/bookings/checkout'; 
    else
        this.configurationService.settingsLoaded$.subscribe(x => {
            this.bookingUrl = this.configurationService.serverSettings.gatewayApiUrl + '/api/v1/b/bookings/checkout';             
        });
  }

  book(product: IProductItem): Observable<boolean> { 
    var guid = Guid.newGuid();

    var data = {
        requestId: guid,
        productId: product.id,
        productName: product.name,
        unitPrice: product.price,
        quantity: 1
    };

    return this.service.postWithId(this.bookingUrl, data).pipe(map((response: Response) => {        
        return true;
    }));
  }
}
