import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

import { SecurityService } from './security.service';
import { ConfigurationService } from './configuration.service';
import {SwPush} from "@angular/service-worker";
import {NewsletterService} from "./newsletter.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  readonly VAPID_PUBLIC_KEY = "BFus7A-uLbVRK7IPlwQ-iUtOYAdrwL0vsFwretaItkxPLVhmLvzYvi9tP1ufzmL1Y34IA2t_u1J5s_NU5esNfWY";

  title = 'Micro Store';
  
  Authenticated: boolean = false;
  subscription: Subscription;
  userName: string = '';

  constructor(      
      private securityService: SecurityService,
      private configurationService: ConfigurationService,
      private swPush: SwPush,
      private newsletterService: NewsletterService
  ) {
      this.Authenticated = this.securityService.IsAuthorized;    
  }

  ngOnInit() {
      console.log('app on init');
      this.subscription = this.securityService.authenticationChallenge$.subscribe(res => {
          this.Authenticated = res;
          console.log('on subscribe: ');
          console.log(this.securityService.UserData);
          this.userName = this.securityService.UserData.name;
      });

      if (window.location.hash) {
          console.log("AuthorizedCallback window.location.hash=" + window.location.hash);
          this.securityService.AuthorizedCallback();
      }

      console.log('identity component, checking authorized' + this.securityService.IsAuthorized);
      this.Authenticated = this.securityService.IsAuthorized;

      if (this.Authenticated) {
          if (this.securityService.UserData) {
              console.log('on init: ');
              console.log(this.securityService.UserData);
              this.userName = this.securityService.UserData.name;
          }
      }

      //Get configuration from server environment variables:
      console.log('configuration');
      this.configurationService.load();        
  }  

  logoutClicked(event: any) {
      event.preventDefault();
      console.log('Logout clicked');
      this.logout();
  }

  login() {
      this.securityService.Authorize();
  }

  logout() {      
      this.securityService.Logoff();
  }

  subscribeToNotifications() {

    this.swPush.requestSubscription({
        serverPublicKey: this.VAPID_PUBLIC_KEY
    })
    .then(sub => this.newsletterService.addPushSubscriber(sub).subscribe())
    .catch(err => console.error("Could not subscribe to notifications", err));
  }

  sendNewsletter() {
    this.newsletterService.send().subscribe();
  }
}
