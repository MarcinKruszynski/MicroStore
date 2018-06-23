
self.addEventListener('push', event => {
    if ("granted" === Notification.permission) {
        var payload = event.data.json(),
            options = {
                body: 'Booking ' + payload.bookingId + ' (' + payload.productId + ' ' + payload.quantity + ') has been paid.',
                icon: 'images/icon.png'                
            };

        event.waitUntil(self.registration.showNotification('New booking has been paid', options));
    }    
});