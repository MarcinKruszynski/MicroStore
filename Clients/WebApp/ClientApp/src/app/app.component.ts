import { Component } from '@angular/core';
import {SwPush} from "@angular/service-worker";
import {NewsletterService} from "./newsletter.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  readonly VAPID_PUBLIC_KEY = "BFus7A-uLbVRK7IPlwQ-iUtOYAdrwL0vsFwretaItkxPLVhmLvzYvi9tP1ufzmL1Y34IA2t_u1J5s_NU5esNfWY";

  title = 'Micro Store';
  user = 'popek365@go2.pl';

  constructor(
    private swPush: SwPush,
    private newsletterService: NewsletterService) {}

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
