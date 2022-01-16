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
        internal OrderDetails CloseOrderDetails(long orderDetailsID,string user)
        {
            try
            {                
                lock (_object)
                {
                    var orderDetails = db.OrderDetails.Where(c=>c.StatusID != 5 && c.OrderDetailsID == orderDetailsID).Include(c => c.UserBranch.Branch).Include("OrderDetailsType").Include("Order").Include(c=>c.Order.UserBranch.Branch).Include("Order.Vehicle").Include("Order.Customer").Include("Order.Customer.CustomerContacts").FirstOrDefault();
                    if (orderDetails is not null && string.IsNullOrEmpty(orderDetails.UsedByUser))
                    {
                        orderDetails.UsedByUser = user;
                        orderDetails.UsedDateTime = DateTime.Now;
                        db.SaveChanges();
                        return orderDetails;
                    }
                    else if (orderDetails is not null && !string.IsNullOrEmpty(orderDetails.UsedByUser) && orderDetails.UsedByUser == user) //equals current user
                    {
                        return orderDetails;
                    }
                    else if (orderDetails is not null && !string.IsNullOrEmpty(orderDetails.UsedByUser) && orderDetails.UsedByUser != user)// not equals current user
                    {
                        return null;
                    }
                    else
                    {
                        return null;
                    }
                }
              

            }
            catch (Exception)
            {

                return null;
            }
        }
        internal long OpenOrderDetails(long orderDetailsID)
        {
            try
            {
                var orderDetails = db.OrderDetails.FirstOrDefault(c => c.OrderDetailsID == orderDetailsID);
                orderDetails.UsedByUser = null;
                orderDetails.UsedDateTime = null;
                db.SaveChanges();
                return orderDetails.OrderDetailsID;
            }
            catch (Exception)
            {

                return 0;
            }
        }

     
    }
}
