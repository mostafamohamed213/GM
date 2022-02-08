"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/NotificationHub").build();
connection.on("SendToUser", (notificationTitle, notificationBody, DTsCreate) => {
    //var heading = document.createElement("h3");
    //heading.textContent = articleHeading;

    //var p = document.createElement("p");
    //p.innerText = articleContent;

    //var div = document.createElement("div");
    //div.appendChild(heading);
    //div.appendChild(p);

    //document.getElementById("articleList").appendChild(div);
    document.getElementById("notbell").style.visibility = "hidden";

    const alertHTML = '<div class="alert">' + notificationTitle + ': ' + notificationBody + '</div>';
    document.body.insertAdjacentHTML('beforeend', alertHTML);
    setTimeout(() => document.querySelector('.alert').classList.add('hide'), 6000);

});

connection.start().catch(function (err) {
    return console.error(err.toString());
}).then(function () {
    connection.invoke('GetConnectionId')
});