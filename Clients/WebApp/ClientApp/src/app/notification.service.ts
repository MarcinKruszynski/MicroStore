import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { catchError, tap } from 'rxjs/operators';
import { Observable, of } from 'rxjs'
import { ConfigurationService } from './configuration.service';

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};


@Injectable({
  providedIn: 'root'
})
export class NotificationService {  

  private notificationsUrl: string = '';  

  constructor(private http: HttpClient, private configurationService: ConfigurationService) {
    if (this.configurationService.isReady)
        this.notificationsUrl = this.configurationService.serverSettings.gatewayApiUrl + '/api/v1/n/notifications'; 
    else
        this.configurationService.settingsLoaded$.subscribe(x => {
            this.notificationsUrl = this.configurationService.serverSettings.gatewayApiUrl + '/api/v1/n/notifications';             
        });
   }

  addPushSubscriber(sub: any) {
    console.log("addPushSubscriber before:", JSON.stringify(sub));

    return this.http.post(this.notificationsUrl, sub, httpOptions);
              /*  .pipe(
                  tap(_ => this.log(`added notification subscription`)),
                  catchError(this.handleError<any>('addPushSubscriber'))
                ); */
  }  

  private handleError<T> (operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
   
      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead
   
      // TODO: better job of transforming error for user consumption
      this.log(`${operation} failed: ${error.message}`);
   
      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }

  private log(message: string) {
    console.log(message);
  }
}
