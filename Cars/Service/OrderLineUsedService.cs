using Cars.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service
{
    public class OrderLineUsedService
    {
        public CarsContext db ;
        public OrderLineUsedService(CarsContext carsContext)
        {
            db = carsContext;
        }
        readonly object _object = new object();
        internal OrderDetails CloseOrderDetails(long orderDetailsID, string user)
        {
            try
            {

                lock (_object)
                {
                    var userRoles = db.UserRoles.Where(c => c.UserId == user).FirstOrDefault();
                    var orderDetails = db.OrderDetails.Where(c => c.StatusID != 5 && c.OrderDetailsID == orderDetailsID).Include(c => c.UserBranch.Branch).Include("OrderDetailsType").Include("Order").Include(c => c.Order.UserBranch.Branch).Include("Order.Vehicle.Brand").Include("Order.Vehicle.BrandModel").Include("Order.Vehicle.ModelYear").Include("Order.Customer").Include("Order.Customer.CustomerContacts").FirstOrDefault();

                    if (userRoles is null || orderDetails is null)
                    {
                        return null;
                    }
                    if (userRoles.RoleId.Trim() == "04e215eb-d6d9-45bd-9d61-e04a81bfb04b".Trim()) //Sales
                    {
                        if (string.IsNullOrEmpty(orderDetails.UsedByUser) && string.IsNullOrEmpty(orderDetails.UsedByUser2))
                        {
                            orderDetails.UsedByUser = user;
                            orderDetails.UsedDateTime = DateTime.Now;
                            orderDetails.UsedByUser2 = user;
                            orderDetails.UsedDateTime2 = DateTime.Now;
                            db.SaveChanges();
                            return orderDetails;
                        }
                        else
                        {
                            if (orderDetails.UsedByUser == user && orderDetails.UsedByUser2 == user)
                            {
                                return orderDetails;
                            }
                            return null;
                        }
                    }
                    else if (userRoles.RoleId.Trim() == "29f2bbf4-3fa6-447a-8d5e-e16e2510d31b".Trim())//Pricing
                    {
                        if (string.IsNullOrEmpty(orderDetails.UsedByUser))
                        {
                            orderDetails.UsedByUser = user;
                            orderDetails.UsedDateTime = DateTime.Now;
                            db.SaveChanges();
                            return orderDetails;
                        }
                        else
                        {
                            if (orderDetails.UsedByUser == user)
                            {
                                return orderDetails;
                            }
                            return null;
                        }
                    }
                    else if (userRoles.RoleId.Trim() == "9c9a27b5-1686-4714-9242-13ffa884fab2".Trim())//Labor
                    {
                        if (string.IsNullOrEmpty(orderDetails.UsedByUser2))
                        {
                            orderDetails.UsedByUser2 = user;
                            orderDetails.UsedDateTime2 = DateTime.Now;
                            db.SaveChanges();
                            return orderDetails;
                        }
                        else
                        {
                            if (orderDetails.UsedByUser2 == user)
                            {
                                return orderDetails;
                            }
                            return null;
                        }

                    }
                    else { return null; }

                }

            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<OrderDetails> BulkCloseOrderDetails (List<long> orderIDs, string userID)
        {
            lock(_object)
            {
                var userRoles = db.UserRoles.Where(c => c.UserId == userID).FirstOrDefault();
                var ordersDetails = db.OrderDetails.Where(c => c.StatusID != 5 && orderIDs.Contains(c.OrderDetailsID))
                    .Include(c => c.UserBranch.Branch).Include("OrderDetailsType").Include("Order")
                    .Include(c => c.Order.UserBranch.Branch).Include("Order.Vehicle.Brand")
                    .Include("Order.Vehicle.BrandModel").Include("Order.Vehicle.ModelYear")
                    .Include("Order.Customer").Include("Order.Customer.CustomerContacts");

                if(userRoles!=null&&ordersDetails!=null&&ordersDetails.Count()>0)
                {
                    if (userRoles.RoleId.Trim() == "04e215eb-d6d9-45bd-9d61-e04a81bfb04b".Trim()) //Sales
                    {
                        //Any Order Used By another user
                        var usedOrders = ordersDetails.Where(x => (!string.IsNullOrEmpty(x.UsedByUser) && x.UsedByUser != userID) || (!string.IsNullOrEmpty(x.UsedByUser2) && x.UsedByUser2 != userID)).ToList();
                        if (usedOrders != null && usedOrders.Count() < 0)
                            return null;
                        foreach (var item in ordersDetails)
                        {
                            item.UsedByUser = userID;
                            item.UsedDateTime = DateTime.Now;
                            item.UsedByUser2 = userID;
                            item.UsedDateTime2 = DateTime.Now;
                        }
                        db.SaveChanges();
                        return ordersDetails;
                    }
                    else if (userRoles.RoleId.Trim() == "29f2bbf4-3fa6-447a-8d5e-e16e2510d31b".Trim())//Pricing
                    {
                        //Any Order Used By another user
                        var usedOrders = ordersDetails.Where(x => (!string.IsNullOrEmpty(x.UsedByUser) && x.UsedByUser != userID)).ToList();
                        if (usedOrders != null && usedOrders.Count() < 0)
                            return null;
                        foreach (var item in ordersDetails)
                        {
                            item.UsedByUser = userID;
                            item.UsedDateTime = DateTime.Now;
                        }
                        db.SaveChanges();
                        return ordersDetails;
                    }
                    else if (userRoles.RoleId.Trim() == "9c9a27b5-1686-4714-9242-13ffa884fab2".Trim())//Labor
                    {
                        //Any Order Used By another user
                        var usedOrders = ordersDetails.Where(x => (!string.IsNullOrEmpty(x.UsedByUser2) && x.UsedByUser2 != userID)).ToList();
                        if (usedOrders != null && usedOrders.Count() < 0)
                            return null;
                        foreach (var item in ordersDetails)
                        {
                            item.UsedByUser2 = userID;
                            item.UsedDateTime2 = DateTime.Now;
                        }
                        db.SaveChanges();
                        return ordersDetails;
                    }
                }
                return null;
            }
        }

        internal long OpenOrderDetails(long orderDetailsID ,string userId)
        {
            try
            {
                var userRoles = db.UserRoles.Where(c => c.UserId == userId).FirstOrDefault();
                var orderDetails = db.OrderDetails.FirstOrDefault(c => c.OrderDetailsID == orderDetailsID);
                if (userRoles is null || orderDetails is null)
                {
                    return 0;
                }
                if (userRoles.RoleId.Trim() == "04e215eb-d6d9-45bd-9d61-e04a81bfb04b".Trim()) //Sales
                {
                    orderDetails.UsedByUser = null;
                    orderDetails.UsedDateTime = null;
                    orderDetails.UsedByUser2 = null;
                    orderDetails.UsedDateTime2 = null;
                    db.SaveChanges();
                    return orderDetails.OrderDetailsID;
                }
                else if (userRoles.RoleId.Trim() == "29f2bbf4-3fa6-447a-8d5e-e16e2510d31b".Trim())//Pricing
                {
                    orderDetails.UsedByUser = null;
                    orderDetails.UsedDateTime = null;
                    db.SaveChanges();
                    return orderDetails.OrderDetailsID;
                }
                else if (userRoles.RoleId.Trim() == "9c9a27b5-1686-4714-9242-13ffa884fab2".Trim())//Labor
                {
                    orderDetails.UsedByUser2 = null;
                    orderDetails.UsedDateTime2 = null;
                    db.SaveChanges();
                    return orderDetails.OrderDetailsID;
                }
                else { return 0; }
                //orderDetails.UsedByUser = null;
                //orderDetails.UsedDateTime = null;
                //db.SaveChanges();
                //return orderDetails.OrderDetailsID;
            }
            catch (Exception)
            {

                return 0;
            }
        }

        public long BulkOpenOrderDetails(List<long> ordersDetailsIDs, string userId)
        {
            try
            {
                var userRoles = db.UserRoles.Where(c => c.UserId == userId).FirstOrDefault();
                var ordersDetails = db.OrderDetails.Where(c => ordersDetailsIDs.Contains(c.OrderDetailsID));
                if (userRoles is null)
                    return 0;
                foreach (var item in ordersDetails)
                {
                    if (userRoles.RoleId.Trim() == "04e215eb-d6d9-45bd-9d61-e04a81bfb04b".Trim()) //Sales
                    {
                        item.UsedByUser = null;
                        item.UsedDateTime = null;
                        item.UsedByUser2 = null;
                        item.UsedDateTime2 = null;
                    }
                    else if (userRoles.RoleId.Trim() == "29f2bbf4-3fa6-447a-8d5e-e16e2510d31b".Trim())//Pricing
                    {
                        item.UsedByUser = null;
                        item.UsedDateTime = null;
                    }
                    else if (userRoles.RoleId.Trim() == "9c9a27b5-1686-4714-9242-13ffa884fab2".Trim())//Labor
                    {
                        item.UsedByUser2 = null;
                        item.UsedDateTime2 = null;
                    }
                    else
                        return 0;
                }
                return db.SaveChanges();
                
            }
            catch (Exception)
            {
                return 0;
            }
        }

        readonly object _object2 = new object();
        //this function shared between pricing and Labor team
        internal OrderDetails ChangeWorkflow(long orderDetailsID,string UserId)
        {
            lock (_object2)
            {
                OrderDetails orderDetails = db.OrderDetails.FirstOrDefault(c=>c.OrderDetailsID == orderDetailsID);
                if (orderDetails is not null)
                {
                    var userRoles = db.UserRoles.Where(c => c.UserId == UserId).FirstOrDefault();

                    if (orderDetails.Price.HasValue && !string.IsNullOrEmpty(orderDetails.PartNumber) && orderDetails.Labor_Hours.HasValue && orderDetails.Labor_Value.HasValue)
                    {
                        orderDetails.WorkflowID = 4;                        
                        WorkflowOrderDetailsLog log = new WorkflowOrderDetailsLog()
                        {
                            WorkflowID = 1,
                            Active = true,
                            DTsCreate = DateTime.Now,
                            OrderDetailsID = orderDetailsID,
                            SystemUserID = UserId,
                            Details = userRoles.RoleId.Trim() == "29f2bbf4-3fa6-447a-8d5e-e16e2510d31b".Trim() ? "From Pricing Team" : "From Labor Team"
                        };
                        db.WorkflowOrderDetailsLogs.Add(log);
                        db.SaveChanges();
                    }
                    else
                    {
                        WorkflowOrderDetailsLog log = new WorkflowOrderDetailsLog()
                        {
                            WorkflowID = 1,
                            Active = true,
                            DTsCreate = DateTime.Now,
                            OrderDetailsID = orderDetailsID,
                            SystemUserID = UserId,
                            Details = userRoles.RoleId.Trim() == "29f2bbf4-3fa6-447a-8d5e-e16e2510d31b".Trim() ? "From Pricing Team" : "From Labor Team"
                        };
                        db.WorkflowOrderDetailsLogs.Add(log);
                        db.SaveChanges();
                    }
                    return orderDetails;
                }
                return null;
            }
        }

        internal void ChangeDTsWorkflowEnter(long orderDetailsID)
        {
            OrderDetails orderDetails = db.OrderDetails.FirstOrDefault(c=>c.OrderDetailsID == orderDetailsID);
            if (orderDetails.Price.HasValue && orderDetails.Labor_Hours.HasValue && orderDetails.Labor_Value.HasValue)
            {
                orderDetails.DTsWorflowEnter = DateTime.Now;
                db.SaveChanges();
            }
        }

        internal OrderDetails CloseOrderDetailsReturned(long orderDetailsID, string user,List<int> status)
        {
            // this function used by pricing and labor team 
            try
            {
                lock (_object)
                {
                    var userRoles = db.UserRoles.Where(c => c.UserId == user).FirstOrDefault();
                    var orderDetails = db.OrderDetails.Where(c => status.Contains(c.StatusID)  && c.OrderDetailsID == orderDetailsID).Include(c => c.UserBranch.Branch).Include("OrderDetailsType").Include("Order").Include(c => c.Order.UserBranch.Branch).Include("Order.Vehicle").Include("Order.Vehicle.Brand").Include("Order.Vehicle.BrandModel").Include("Order.Vehicle.ModelYear").FirstOrDefault();

                    if (userRoles is null || orderDetails is null)
                    {
                        return null;
                    }       
                    else if (userRoles.RoleId.Trim() == "29f2bbf4-3fa6-447a-8d5e-e16e2510d31b".Trim())//Pricing
                    {
                        if (string.IsNullOrEmpty(orderDetails.UsedByUser))
                        {
                            orderDetails.UsedByUser = user;
                            orderDetails.UsedDateTime = DateTime.Now;
                            db.SaveChanges();
                            return orderDetails;
                        }
                        else
                        {
                            if (orderDetails.UsedByUser == user)
                            {
                                return orderDetails;
                            }
                            return null;
                        }
                    }
                    else if (userRoles.RoleId.Trim() == "9c9a27b5-1686-4714-9242-13ffa884fab2".Trim())//Labor
                    {
                        if (string.IsNullOrEmpty(orderDetails.UsedByUser2))
                        {
                            orderDetails.UsedByUser2 = user;
                            orderDetails.UsedDateTime2 = DateTime.Now;
                            db.SaveChanges();
                            return orderDetails;
                        }
                        else
                        {
                            if (orderDetails.UsedByUser2 == user)
                            {
                                return orderDetails;
                            }
                            return null;
                        }
                    }
                    else { return null; }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
