﻿/// <reference path="oidc-client.js" />

var pubKey = 'BEoYccO5q2KhdfX1Wt0U4tpWzZBRa4FGImrvzXqu073QmO2V7r3aGv0MOf-BizbWX5V3H65ns3p7uZ07DMMrjvk';
// wD97u6MzPoW77gtRmOiu0gpNON7wE9zEJIOXyGizO2c

var notifyBtn = document.getElementById("notify");
notifyBtn.disabled = true;

let isSubscribed = false;
let swRegistration = null;

function urlB64ToUint8Array(base64String) {
    const padding = '='.repeat((4 - base64String.length % 4) % 4);
    const base64 = (base64String + padding)
        .replace(/\-/g, '+')
        .replace(/_/g, '/');

    const rawData = window.atob(base64);
    const outputArray = new Uint8Array(rawData.length);

    for (let i = 0; i < rawData.length; ++i) {
        outputArray[i] = rawData.charCodeAt(i);
    }
    return outputArray;
}

if ('serviceWorker' in navigator && 'PushManager' in window) {
    if (Notification.permission !== 'denied') {
        notifyBtn.disabled = false;
    }

    navigator.serviceWorker.register('sw.js').then(swReg => { 
        swRegistration = swReg;

        swRegistration.pushManager.getSubscription()
            .then(subscription => {
                isSubscribed = !(subscription === null);                
            });
    });
}

notifyBtn.addEventListener('click', function (evt) {
    this.disabled = true;

    swRegistration.pushManager.getSubscription().then(s => {
        if (s !== null) {
            s.unsubscribe();

            this.disabled = false;
        } else {
            swRegistration.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: urlB64ToUint8Array(pubKey)
            })
            .then(s => {
                //fetch('api/subscription', {
                //    headers: { 'Content-Type': 'application/json' },
                //    method: 'POST',
                //    body: JSON.stringify(s)
                //});

                //test
                var el = document.getElementById("test"), elClone = el.cloneNode(true);
                el.parentNode.replaceChild(elClone, el);

                var testBtn = document.getElementById("test");

                testBtn.addEventListener('click', function () {
                    fetch('http://localhost:5104/api/v1/notifications', {
                        method: 'post',
                        headers: {
                            'Content-type': 'application/json'
                        },
                        body: JSON.stringify(s)
                    });
                });
            })
            .then(res => {
                this.disabled = false;
            });
        }
    });
});





function log() {
    document.getElementById('results').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        document.getElementById('results').innerHTML += msg + '\r\n';
    });
}

document.getElementById("login").addEventListener("click", login, false);
document.getElementById("api").addEventListener("click", api, false);
document.getElementById("checkout").addEventListener("click", checkout, false);
document.getElementById("logout").addEventListener("click", logout, false);

/*
var mgr;
var serverSettings;

const baseURI = document.baseURI.endsWith('/') ? document.baseURI : `${document.baseURI}/`;
var url = `${baseURI}Home/Configuration`;

var xhr = new XMLHttpRequest();
xhr.open("GET", url);
xhr.onload = function () {
    console.log('server settings loaded');
    var serverSettings = JSON.parse(xhr.responseText);
    console.log(serverSettings);

    if (!serverSettings)
        return;

    var config = {
        authority: serverSettings.identityUrl,
        client_id: "spa",
        redirect_uri: baseURI + "callback.html",
        response_type: "id_token token",
        scope: "openid profile products",
        post_logout_redirect_uri: baseURI + "index.html"
    };

    mgr = new Oidc.UserManager(config);

    mgr.getUser().then(function (user) {
        if (user) {
            log("User logged in", user.profile);
        }
        else {
            log("User not logged in");
        }
    });
};
xhr.send();
*/

var config = {
    authority: "http://localhost:5201",
    client_id: "spa",
    redirect_uri: "http://localhost:5100/callback.html",
    response_type: "id_token token",
    scope: "openid profile products booking",
    post_logout_redirect_uri: "http://localhost:5100/index.html",
};
var mgr = new Oidc.UserManager(config);

mgr.getUser().then(function (user) {
    if (user) {
        log("User logged in", user.profile);
    }
    else {
        log("User not logged in");
    }
});


function login() {
    if (mgr)
        mgr.signinRedirect();
}

function api() {
    if (mgr /*&& serverSettings*/)
        mgr.getUser().then(function (user) {
            //var url = serverSettings.productUrl + "/api/v1/products";
            var url = "http://localhost:5200/api/v1/p/products";

            var xhr = new XMLHttpRequest();
            xhr.open("GET", url);
            xhr.onload = function () {
                log(xhr.status, JSON.parse(xhr.responseText));
            };
            xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
            xhr.send();
        });
}

function newGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function checkout() {
    if (mgr)
        mgr.getUser().then(function (user) {            
            var url = "http://localhost:5200/api/v1/b/bookings/checkout";

            var guid = newGuid();

            var data = {
                requestId: guid,
                productId: 1,
                productName: "Progressive apps - 31.06.2018",
                unitPrice: 404,
                quantity: 1
            };

            var xhr = new XMLHttpRequest();
            xhr.open("POST", url);            
            xhr.onreadystatechange = function () {
                if (xhr.readyState == XMLHttpRequest.DONE) {                    
                }
            };
            
            xhr.setRequestHeader("Content-type", "application/json");            
            xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
            xhr.send(JSON.stringify(data));
        });
}

function logout() {
    if (mgr)
        mgr.signoutRedirect();
}