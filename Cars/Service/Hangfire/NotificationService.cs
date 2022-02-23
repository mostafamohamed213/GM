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
        private CarsContext _context;
        private UserService _userService;
        private OrderDetailsService _orderDetailsService;
        private Cars.Service.NotificationService _notificationService;
        public NotificationService(CarsContext carsContext, UserService userService, OrderDetailsService orderDetailsService,
            Cars.Service.NotificationService notificationService)
        {
            _context = carsContext;
            _userService = userService;
            _orderDetailsService = orderDetailsService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Used to send notification to users "SignalR, Mail" At Hangfire Service 
        /// </summary>
        /// <returns></returns>
        public async Task SendNotificationsAsync()
        {
            List<OrderDetails> ordersToUpdate = new List<OrderDetails>();
            //Get All Teams Duration 
            var durations = await _context.TeamDurations.Include(x => x.IdentityRole).Include(x => x.TeamMembersAllowed).ThenInclude(x => x.User).ToListAsync();
            if (durations == null || durations.Count() <= 0)
                return;

            //Sales
            var salesDuration = durations.Where(x => x.IdentityRole?.Name?.ToLower() == "sales").FirstOrDefault();
            if (salesDuration != null)
            {
                var salesOrders = await GetSalesOrderDetailsAsync(1, salesDuration.Duration);
                if (salesOrders != null && salesOrders.Count() > 0)
                {
                    await SendNotificationAndEmailForWorkflowAsync(salesOrders, salesDuration, "Sales");
                    ordersToUpdate.AddRange(salesOrders);
                }
            }

            //Pricing
            var pricingDuration = durations.Where(x => x.IdentityRole?.Name?.ToLower() == "pricing").FirstOrDefault();
            if (pricingDuration != null)
            {
                var pricingOrders = await GetPricingOrderDetailsAsync(2, 1, pricingDuration.Duration);
                if (pricingOrders != null && pricingOrders.Count() > 0)
                {
                    await SendNotificationAndEmailForWorkflowAsync(pricingOrders, pricingDuration, "Pricing");
                    ordersToUpdate.AddRange(pricingOrders);
                }
            }

            //Labor
            var laborDuration = durations.Where(x => x.IdentityRole?.Name?.ToLower() == "labor").FirstOrDefault();
            if (laborDuration != null)
            {
                var laborOrders = await GetLaborOrderDetailsAsync(2, 1, laborDuration.Duration);
                if (laborOrders != null && laborOrders.Count() > 0)
                {
                    await SendNotificationAndEmailForWorkflowAsync(laborOrders, laborDuration, "Labor");
                    ordersToUpdate.AddRange(laborOrders);
                }
            }

            //qoutation
            var qoutationDuration = durations.Where(x => x.IdentityRole?.Name?.ToLower() == "qoutation").FirstOrDefault();
            if (qoutationDuration != null)
            {
                var qoutationOrders = await GetOrderDetailsAsync(2, 4, qoutationDuration.Duration);
                if (qoutationOrders != null && qoutationOrders.Count() > 0)
                {
                    await SendNotificationAndEmailForWorkflowAsync(qoutationOrders, qoutationDuration, "Qoutation");
                    ordersToUpdate.AddRange(qoutationOrders);
                }
            }

            //Finance
            var financeDuration = durations.Where(x => x.IdentityRole?.Name?.ToLower() == "finance").FirstOrDefault();
            if (financeDuration != null)
            {
                var financeOrders = await GetOrderDetailsAsync(2, 5, financeDuration.Duration);
                if (financeOrders != null && financeOrders.Count() > 0)
                {
                    await SendNotificationAndEmailForWorkflowAsync(financeOrders, financeDuration, "Finance");
                    ordersToUpdate.AddRange(financeOrders);
                }
            }

            //Purchasing
            var purchasingDuration = durations.Where(x => x.IdentityRole?.Name?.ToLower() == "purchasing").FirstOrDefault();
            if (purchasingDuration != null)
            {
                var purchasingOrders = await GetOrderDetailsAsync(2, 6, purchasingDuration.Duration);
                if (purchasingOrders != null && purchasingOrders.Count() > 0)
                {
                    await SendNotificationAndEmailForWorkflowAsync(purchasingOrders, purchasingDuration, "Purchasing");
                    ordersToUpdate.AddRange(purchasingOrders);
                }
            }

            //Inventory
            var inventoryDuration = durations.Where(x => x.IdentityRole?.Name?.ToLower() == "inventory").FirstOrDefault();
            if (inventoryDuration != null)
            {
                var inventoryOrders = await GetOrderDetailsAsync(2, 7, inventoryDuration.Duration);
                if (inventoryOrders != null && inventoryOrders.Count() > 0)
                {
                    await SendNotificationAndEmailForWorkflowAsync(inventoryOrders, inventoryDuration, "Inventory");
                    ordersToUpdate.AddRange(inventoryOrders);
                }
            }


            //Runner - Specific
            var runnerDuration = durations.Where(x => x.IdentityRole?.Name?.ToLower() == "runner").FirstOrDefault();
            if (runnerDuration != null)
            {
                var runnerOrders = await GetRunnerOrderDetailsAsync(runnerDuration.Duration);
                if (runnerOrders != null && runnerOrders.Count() > 0)
                {
                    await SendRunnerNotificationAndEmailForWorkflowAsync(runnerOrders, runnerDuration);
                    ordersToUpdate.AddRange(runnerOrders);
                }
            }

            //Update NotificationSent = True
            ordersToUpdate = ordersToUpdate.Select(x =>
            {
                x.NotificationSent = true;
                return x;
            }).ToList();
            _context.OrderDetails.UpdateRange(ordersToUpdate);
            await _context.SaveChangesAsync();

        }

        /// <summary>
        /// Get All Orders Which Added to workflow after spesific duration 
        /// </summary>
        /// <param name="statusID"></param>
        /// <param name="workflowID"></param>
        /// <param name="duration">In Hours</param>
        /// <returns></returns>
        private async Task<IEnumerable<OrderDetails>> GetOrderDetailsAsync(int statusID, int workflowID, double duration)
        {
            try
            {
                var orderDetails = await _context.OrderDetails.Where(c => c.StatusID == statusID && c.WorkflowID == workflowID
                                                                            && c.DTsWorflowEnter <= DateTime.UtcNow.AddHours(-1 * duration)
                                                                            && c.NotificationSent != true).ToListAsync();
                return orderDetails;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get All Sales Orders Which Added to workflow after spesific duration 
        /// </summary>
        /// <param name="statusID"></param>
        /// <param name="workflowID"></param>
        /// <param name="duration">In Hours</param>
        /// <returns></returns>
        private async Task<IEnumerable<OrderDetails>> GetSalesOrderDetailsAsync(int workflowID, double duration)
        {
            try
            {
                var orderDetails = await _context.OrderDetails.Where(c => c.StatusID != 1 && c.StatusID != 5 && c.WorkflowID == workflowID
                                                                            && c.DTsWorflowEnter <= DateTime.UtcNow.AddHours(-1 * duration)
                                                                            && c.NotificationSent != true).ToListAsync();
                return orderDetails;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get All Pricing Orders Which Added to workflow after spesific duration 
        /// </summary>
        /// <param name="statusID"></param>
        /// <param name="workflowID"></param>
        /// <param name="duration">In Hours</param>
        /// <returns></returns>
        private async Task<IEnumerable<OrderDetails>> GetPricingOrderDetailsAsync(int statusID, int workflowID, double duration)
        {
            try
            {
                var orderDetails = await _context.OrderDetails.Where(c => c.StatusID == statusID && c.WorkflowID == workflowID && !c.Price.HasValue
                                                                            && c.DTsWorflowEnter <= DateTime.UtcNow.AddHours(-1 * duration)
                                                                            && c.NotificationSent != true).ToListAsync();
                return orderDetails;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get All Labor Orders Which Added to workflow after spesific duration 
        /// </summary>
        /// <param name="statusID"></param>
        /// <param name="workflowID"></param>
        /// <param name="duration">In Hours</param>
        /// <returns></returns>
        private async Task<IEnumerable<OrderDetails>> GetLaborOrderDetailsAsync(int statusID, int workflowID, double duration)
        {
            try
            {
                var orderDetails = await _context.OrderDetails.Where(c => c.StatusID == statusID && c.WorkflowID == workflowID && c.Order.WithMaintenance.HasValue && c.Order.WithMaintenance.Value
                                                                            && c.DTsWorflowEnter <= DateTime.UtcNow.AddHours(-1 * duration)
                                                                            && c.NotificationSent != true).ToListAsync();
                return orderDetails;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get All Runners Orders Which Added to workflow after spesific duration 
        /// </summary>
        /// <param name="duration">In Hours</param>
        /// <returns></returns>
        private async Task<IEnumerable<OrderDetails>> GetRunnerOrderDetailsAsync(double duration)
        {
            try
            {
                var orderDetails = await _context.OrderDetails.Where(c => (c.RunnerID != null && !string.IsNullOrEmpty(c.RunnerID))
                                                                            && c.DTsWorflowEnter <= DateTime.UtcNow.AddHours(-1 * duration)
                                                                            && c.NotificationSent != true).ToListAsync();
                return orderDetails;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task SendNotificationAndEmailForWorkflowAsync(IEnumerable<OrderDetails> orders, TeamDuration duration, string teamName)
        {
            try
            {
                foreach (var order in orders)
                {
                    List<ApplicationUser> usersToSend = new List<ApplicationUser>();

                    //Get Users To send Notifications to
                    if (!string.IsNullOrEmpty(order.UsedByUser))
                    {
                        var userUseOrder = await _userService.GetByIDAsync(order.UsedByUser);
                        if (userUseOrder != null)
                            usersToSend.Add(userUseOrder);

                        if (duration.TeamMembersAllowed != null && duration.TeamMembersAllowed.Count() > 0)
                        {
                            var usersAllowedToSend = duration.TeamMembersAllowed.Where(x => x.isAssigned == true).Select(x => x.User).ToList();
                            if (usersAllowedToSend != null && usersAllowedToSend.Count() > 0)
                                usersToSend.AddRange(usersAllowedToSend);
                        }
                    }
                    else
                    {
                        if (duration.TeamMembersAllowed != null && duration.TeamMembersAllowed.Count() > 0)
                        {
                            var usersAllowedToSend = duration.TeamMembersAllowed.Where(x => x.isAssigned != true).Select(x => x.User).ToList();
                            if (usersAllowedToSend != null && usersAllowedToSend.Count() > 0)
                                usersToSend.AddRange(usersAllowedToSend);
                        }
                    }

                    //Send Notifications
                    if (usersToSend != null && usersToSend.Count() > 0)
                        await _notificationService.SendNotificationAndEmailForDelayedOrderAsnc(usersToSend, "Delayed Order", $"There is a delayed order at {teamName} with ID : " + order.Prefix, order.Prefix, teamName);
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private async Task SendRunnerNotificationAndEmailForWorkflowAsync(IEnumerable<OrderDetails> orders, TeamDuration duration)
        {
            try
            {
                foreach (var order in orders)
                {
                    List<ApplicationUser> usersToSend = new List<ApplicationUser>();
                    var userUseOrder = await _userService.GetByIDAsync(order.RunnerID);
                    if (userUseOrder != null)
                        usersToSend.Add(userUseOrder);

                    if (duration.TeamMembersAllowed != null && duration.TeamMembersAllowed.Count() > 0)
                    {
                        var usersAllowedToSend = duration.TeamMembersAllowed.Where(x => x.isAssigned == true).Select(x => x.User).ToList();
                        if (usersAllowedToSend != null && usersAllowedToSend.Count() > 0)
                            usersToSend.AddRange(usersAllowedToSend);
                    }

                    //Send Notifications
                    if (usersToSend != null && usersToSend.Count() > 0)
                        await _notificationService.SendNotificationAndEmailForDelayedOrderAsnc(usersToSend, "Delayed Order", "There is a delayed order for runner with ID : " + order.Prefix, order.Prefix,"Runner");
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}

