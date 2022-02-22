using Cars.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service.Hangfire
{
    public class NotificationService : INotificationService
    {
        private  CarsContext _context { get; set; }
        private UserService _userService { get; set; }
        private OrderDetailsService _orderDetailsService { get; set; }

        public NotificationService(CarsContext carsContext, UserService userService, OrderDetailsService orderDetailsService)
        {
            _context = carsContext;
            _userService = userService;
            _orderDetailsService = orderDetailsService;
        }

        /// <summary>
        /// Used to send notification to users "SignalR, Mail" At Hangfire Service 
        /// </summary>
        /// <returns></returns>
        public async Task SendNotificationsAsync()
        {
            //Get All Teams Duration 
            var durations = await _context.TeamDurations.Include(x => x.TeamMembersAllowed).ThenInclude(x => x.Users).ToListAsync();
            if (durations == null || durations.Count() <= 0)
                return;
 
            //Pricing
            //Get All Orders For Pricing
            //var pricingOrders = await  _orderDetailsService.GetOrderDetailsAsync

            //Labor 
            //qoutation
            //Finance
            //Purchasing
            //Runner - Specific
            //Inventory
            //Delivery - Specific
            //Sales
        }
    }
}

