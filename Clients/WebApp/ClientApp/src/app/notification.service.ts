import { Injectable, Inject } from '@angular/core';
import { DataService } from './data.service';
import { Observable, of } from 'rxjs'
import { ConfigurationService } from './configuration.service';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {  

  private notificationsUrl: string = '';  

  constructor(private service: DataService, private configurationService: ConfigurationService) {
    if (this.configurationService.isReady)
        this.notificationsUrl = this.configurationService.serverSettings.gatewayApiUrl + '/api/v1/n/notifications'; 
    else
        this.configurationService.settingsLoaded$.subscribe(x => {
            this.notificationsUrl = this.configurationService.serverSettings.gatewayApiUrl + '/api/v1/n/notifications';             
        });
   }  

  addPushSubscriber(sub: any): Observable<boolean> { 
    console.log("addPushSubscriber before:", JSON.stringify(sub));

    return this.service.postWithId(this.notificationsUrl, sub).pipe(map((response: Response) => {        
        return true;
    }));
  }
}
