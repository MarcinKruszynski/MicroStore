import { Injectable } from '@angular/core';

import { DataService } from './data.service';
import { ConfigurationService } from './configuration.service';

@Injectable({
  providedIn: 'root'
})
export class BookingService {

  private bookingsUrl: string = '';  

  constructor(private service: DataService, private configurationService: ConfigurationService) {
    if (this.configurationService.isReady)
        this.bookingsUrl = this.configurationService.serverSettings.gatewayApiUrl + '/api/v1/b/bookings/checkout'; 
    else
        this.configurationService.settingsLoaded$.subscribe(x => {
            this.bookingsUrl = this.configurationService.serverSettings.gatewayApiUrl + '/api/v1/b/bookings/checkout';             
        });
  }

  checkout() {

  }
}
