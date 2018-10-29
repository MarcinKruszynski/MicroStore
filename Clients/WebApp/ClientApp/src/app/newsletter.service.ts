import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { catchError, tap } from 'rxjs/operators';
import { Observable, of } from 'rxjs'

const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};


@Injectable({
  providedIn: 'root'
})
export class NewsletterService {  

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) { }

  addPushSubscriber(sub: any) {
    console.log("addPushSubscriber before:", JSON.stringify(sub));

    return this.http.post(this.baseUrl + 'api/notifications', sub, httpOptions);
              /*  .pipe(
                  tap(_ => this.log(`added notification subscription`)),
                  catchError(this.handleError<any>('addPushSubscriber'))
                ); */
  }

  send() {
    return this.http.post(this.baseUrl + 'api/newsletter', {}, httpOptions);
              /*  .pipe(
                  tap(_ => this.log(`sent notification`)),
                  catchError(this.handleError<any>('send'))
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
