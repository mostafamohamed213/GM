using Cars.Consts;
using Cars.Models;
using Cars.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service
{
    public class PricingService
    {
        public CarsContext db { get; set; }
        public PricingService(CarsContext carsContext)
        {
            db = carsContext;
        }

        public PagingViewModel<OrderDetails> getOrders(int currentPage)
        {
            var orders = db.OrderDetails.Include("OrderDetailsType").Where(c => c.StatusID == 2 && c.WorkflowID == 2).Skip((currentPage - 1) * TablesMaxRows.IndexPricingMaxRows).Take(TablesMaxRows.IndexPricingMaxRows).ToList();

            PagingViewModel<OrderDetails> viewModel = new PagingViewModel<OrderDetails>();         
            viewModel.items = orders.ToList();
            var itemsCount = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 2).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexPricingMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexPricingMaxRows;
            return viewModel;
        }
        public PagingViewModel<OrderDetails> getOrdersWithChangelength(int currentPageIndex, int length)
        {
            TablesMaxRows.IndexPricingMaxRows = length;
            return getOrders(currentPageIndex);
        }
        readonly object _object = new object();
        //internal OrderDetails GetOrderDetailsByOrderDetailsID(long orderDetailsID)
        //{          
        //    lock (_object)
        //    {
        //        var orderDetails = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 2 && c.OrderDetailsID == orderDetailsID).Include("Order").Include("Order.Vehicle").Include("Order.Customer").Include("Order.Customer.CustomerContacts").FirstOrDefault();
        //        if (orderDetails is not null && string.IsNullOrEmpty(orderDetails.UsedByUser))
        //        {
        //            orderDetails.UsedByUser = "1";
        //            orderDetails.UsedDateTime = DateTime.Now;
        //            db.SaveChanges();
        //            return orderDetails;
        //        }
        //        else if (orderDetails is not null && !string.IsNullOrEmpty(orderDetails.UsedByUser) && orderDetails.UsedByUser == "1") //equals current user
        //        {
        //            return orderDetails;
        //        }
        //        else if (orderDetails is not null && !string.IsNullOrEmpty(orderDetails.UsedByUser) && orderDetails.UsedByUser != "1")// not equals current user
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }         
        //}

        internal SelectList GetSelectListVendorLocations()
        {
            var VendorLocations = db.VendorLocations.ToList();
            if (VendorLocations.Count() > 0)
            {
                return new SelectList(VendorLocations, "VendorLocationID", "NameEn");
            }
            return null;
        }
        internal List<OrderDetails> GetOrderDeatilsChildren(long parentID)
        {
            var Children= db.OrderDetails.Include("OrderDetailsType").Include("VendorLocation").Where(c => c.ParentOrderDetailsID.HasValue && c.ParentOrderDetailsID.Value == parentID).ToList();
            return Children;
        }

        internal int AddPricingField(long orderDetailsID, string partNumber, decimal price, int vendorLocationID, string comments)
        {
            try
            {
                var orderDetails = db.OrderDetails.Include("Order").FirstOrDefault(c => c.OrderDetailsID == orderDetailsID);
                if (orderDetails is null)
                {
                    return -1;
                }
                orderDetails.PartNumber = partNumber;
                orderDetails.Price = price;
                orderDetails.VendorLocationID = vendorLocationID;
                if (!string.IsNullOrWhiteSpace(comments))
                {
                    orderDetails.Comments = comments;
                }
                if (orderDetails.Order.WithMaintenance.HasValue && orderDetails.Order.WithMaintenance.Value)
                {
                    orderDetails.WorkflowID = 3;
                }
                else
                {
                    orderDetails.WorkflowID = 4;
                }
               
               
                WorkflowOrderDetailsLog log = new WorkflowOrderDetailsLog()
                {
                    WorkflowID = 2,
                    Active = true,
                    DTsCreate = DateTime.Now,
                    OrderDetailsID = orderDetailsID,
                    SystemUserID = "1"
                };
                db.WorkflowOrderDetailsLogs.Add(log);
                db.SaveChanges();
                return 1;
            }
            catch (Exception)
            {

                return -1;
            }
        
        }
        internal OrderDetails GetOrderDetailsByOrderDetailsID(long orderDetailsID)
        {
            var orderDetails = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 2 && c.OrderDetailsID == orderDetailsID).Include("OrderDetailsType").FirstOrDefault();
            orderDetails.Children = db.OrderDetails.Where(c=> c.ParentOrderDetailsID.HasValue && c.ParentOrderDetailsID == orderDetails.OrderDetailsID).ToList();
            return orderDetails;
        }

        internal OrderDetails AddOrderLine(PricingAddOrderLineViewModel model)
        {
            try {
                var parent = db.OrderDetails.Include("Order").FirstOrDefault(c => c.OrderDetailsID == model.ParentOrderDetailsId);
                OrderDetails orderDetails = new OrderDetails
                {
                    Comments = model.Comments,
                    VendorLocationID = model.VendorLocationID,
                    OrderDetailsTypeID = model.TypeID,
                    Price = model.Price,
                    PartNumber = model.PartNumber,                   
                    Quantity = parent.Quantity,
                    OrderID = parent.OrderID,
                    Items = parent.Items,
                    IsApproved = parent.IsApproved,                   
                    ParentOrderDetailsID = parent.OrderDetailsID,
                    DTsCreate = DateTime.Now,
                    SystemUserCreate = "1",
                    StatusID = 2,
                    BranchID = 1,
                };
                if (parent.Order.WithMaintenance.HasValue && parent.Order.WithMaintenance.Value)
                {
                    orderDetails.WorkflowID = 3;
                }
                else
                {
                    orderDetails.WorkflowID = 4;
                }
                db.OrderDetails.Add(orderDetails);
                db.SaveChanges();
                OrderDetailsStatusLog statusLog = new OrderDetailsStatusLog()
                {
                    DTsCreate = DateTime.Now,
                    OrderDetailsID = orderDetails.OrderDetailsID,
                    StatusID = 2,
                    SystemUserID = "1"
                };
                db.OrderDetailsStatusLogs.Add(statusLog);
                WorkflowOrderDetailsLog workflowLog = new WorkflowOrderDetailsLog()
                {
                    DTsCreate =DateTime.Now,
                    SystemUserID="1",
                    OrderDetailsID = orderDetails.OrderDetailsID,
                    WorkflowID = 2,
                    Active=true
                };
                db.WorkflowOrderDetailsLogs.Add(workflowLog);
                db.SaveChanges();
                return parent;
            }
            catch (Exception)
            {
                return null;
            }
         
        }

        internal SelectList GetSelectListOrderDetailsType()
        {
            var OrderDetailsTypes = db.OrderDetailsType.Where(c => !c.NameEn.Contains("Any")).ToList();
            if (OrderDetailsTypes.Count() > 0)
            {
                return new SelectList(OrderDetailsTypes, "OrderDetailsTypeID", "NameEn");
            }
            return null;
        }
    }
}
