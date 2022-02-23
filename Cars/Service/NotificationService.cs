using Cars.Models;
using Cars.Service.Email;
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
        private readonly IEmailService _emailService;
        private readonly IHubContext<NotificationHubService> _notificationHubContext;
        public NotificationService(CarsContext carsContext, IUserConnectionManager userConnectionManager, IHubContext<NotificationHubService> notificationHubContext,
            IEmailService emailService)
        {
            _context = carsContext;
            _userConnectionManager = userConnectionManager;
            _notificationHubContext = notificationHubContext;
            _emailService = emailService;
        }

        public async Task SendNotificationAndEmailForAddedOrderAsnc(IEnumerable<ApplicationUser> users, string notificationTitle, string notificationBody, string orderPrefix)
        {
            try
            {
                //Send Signal R Notification
                await AddAndSendNotificationAsnc(users, notificationTitle, notificationBody);

                //By Email 
                foreach (var user in users)
                    _emailService.SendEmailForAddedOrderDetails(orderPrefix, user.Email);
            }
            catch (Exception)
            {
                return;
            }
        }

        public async Task SendNotificationAndEmailForDelayedOrderAsnc(IEnumerable<ApplicationUser> users, string notificationTitle, string notificationBody, string orderPrefix, string teamName)
        {
            try
            {
                //Send Signal R Notification
                await AddAndSendNotificationAsnc(users, notificationTitle, notificationBody);

                //By Email 
                foreach (var user in users)
                    _emailService.SendEmailForDelayedOrderDetails(orderPrefix, teamName, user.Email);
            }
            catch (Exception)
            {
                return;
            }
        }

        public async Task SendNotificationAndEmailForRejectedOrderAsnc(IEnumerable<ApplicationUser> users, string notificationTitle, string notificationBody, string orderPrefix, string teamName)
        {
            try
            {
                //Send Signal R Notification
                await AddAndSendNotificationAsnc(users, notificationTitle, notificationBody);

                //By Email 
                foreach (var user in users)
                    _emailService.SendEmailForRejectedOrderDetails(orderPrefix, teamName, user.Email);
            }
            catch (Exception)
            {
                return;
            }
        }

        public async Task<Cars.Models.Notification> AddAndSendNotificationAsnc(IEnumerable<ApplicationUser> users, string notificationTitle, string notificationBody)
        {
            //Add Notification and User Notification 
            //var notification = await AddAsync(new Models.Notification()
            //{
            //    Title = notificationTitle,
            //    Description = notificationBody,
            //    DTsCreate = DateTime.UtcNow,
            //    NotificationUser = userIDs.Select(x => new NotificationUser()
            //    {
            //        DTsCreate = DateTime.UtcNow,
            //        UserID = x,
            //        IsRead = false
            //    }).ToList()
            //});
            //if (notification == null || notification.NotificationID <= 0)
            //    return null;

            var notification = new Models.Notification()
            {
                Title = notificationTitle,
                Description = notificationBody,
                DTsCreate = DateTime.UtcNow
            };
            
            //Get Users Connections
            List<string> connections = new List<string>();
            foreach (var user in users)
            {
                connections.AddRange(_userConnectionManager.GetUserConnections(user.Id));
            }

            //Send To Users Connections By Signal R
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
