/// <reference path="oidc-client.js" />

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
document.getElementById("logout").addEventListener("click", logout, false);

var mgr;
var serverSettings;

const baseURI = document.baseURI.endsWith('/') ? document.baseURI : `${document.baseURI}/`;
var url = `${baseURI}Home/Configuration`;

var xhttp = new XMLHttpRequest();
xhttp.onreadystatechange = function () {
    if (this.readyState == 4 && this.status == 200) {
        console.log('server settings loaded');
        var serverSettings = response.json();
        console.log(serverSettings);

        if (!serverSettings)
            return;

        var config = {
            authority: serverSettings.identityUrl,
            client_id: "spa",
            redirect_uri: baseURI + "/callback.html",
            response_type: "id_token token",
            scope: "openid profile products",
            post_logout_redirect_uri: baseURI + "/index.html"
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
    }
};
xhttp.open("GET", url, true);
xhttp.send();


function login() {
    if (mgr)
        mgr.signinRedirect();
}

function api() {
    if (mgr && serverSettings)
        mgr.getUser().then(function (user) {
            var url = serverSettings.productUrl + "/api/v1/products";

            var xhr = new XMLHttpRequest();
            xhr.open("GET", url);
            xhr.onload = function () {
                log(xhr.status, JSON.parse(xhr.responseText));
            };
            xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
            xhr.send();
        });
}

function logout() {
    if (mgr)
        mgr.signoutRedirect();
}