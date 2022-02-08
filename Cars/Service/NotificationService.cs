using Cars.Models;
using Cars.Service.Notification;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service
{
    public class NotificationService
    {
        public CarsContext _context { get; set; }
        private readonly IUserConnectionManager _userConnectionManager;
        private readonly IHubContext<NotificationHubService> _notificationHubContext;
        public NotificationService(CarsContext carsContext, IUserConnectionManager userConnectionManager, IHubContext<NotificationHubService> notificationHubContext)
        {
            _context = carsContext;
            _userConnectionManager = userConnectionManager;
            _notificationHubContext = notificationHubContext;
        }
        
        public async Task<Cars.Models.Notification> AddAndSendNotificationAsnc(List<string> userIDs, string notificationTitle, string notificationBody)
        {
            //Add Notification and User Notification 
            var notification = await AddAsync(new Models.Notification()
            {
                Title = notificationTitle,
                Description = notificationBody,
                DTsCreate = DateTime.UtcNow,
                NotificationUser = userIDs.Select(x => new NotificationUser()
                {
                    DTsCreate = DateTime.UtcNow,
                    UserID = x,
                    IsRead = false
                }).ToList()
            });
            if (notification == null || notification.NotificationID <= 0)
                return null;

            //Get Users Connections
            List<string> connections = new List<string>();
            foreach (var userID in userIDs)
            {
                connections.AddRange(_userConnectionManager.GetUserConnections(userID));
            }

            //Send To Users Connections 
            if (connections != null && connections.Count > 0)
            {
                foreach (var connectionId in connections)
                {
                    await _notificationHubContext.Clients.Client(connectionId).SendAsync("sendToUser", notificationTitle, notificationBody, notification.DTsCreate.ToString("ddd, dd MMM yyyy hh:mm tt"));
                }
            }

            return notification;
        }

        private async Task<Cars.Models.Notification> AddAsync(Cars.Models.Notification model)
        {
            var result = await _context.Notification.AddAsync(model);
            if (await _context.SaveChangesAsync() > 0)
                return result.Entity;
            else
                return null;
        }
    }
}
