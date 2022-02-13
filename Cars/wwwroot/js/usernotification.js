"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/NotificationHub").build();
connection.on("SendToUser", (notificationTitle, notificationBody, DTsCreate) => {
    toastr.info(notificationBody, notificationTitle, { timeOut: 6000 });
});

connection.start().catch(function (err) {
    return console.error(err.toString());
}).then(function () {
    connection.invoke('GetConnectionId')
});