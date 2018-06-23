
self.addEventListener('push', event => {
    if ("granted" === Notification.permission) {
        var payload = event.data.json(),
            options = {
                body: payload.productName,
                icon: 'images/icon.png'                
            };

        event.waitUntil(self.registration.showNotification('Booking ' + payload.bookingId + ' has been paid', options));
    }    
});