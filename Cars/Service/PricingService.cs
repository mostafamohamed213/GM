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
        public OrderLineUsedService usedService { get; set; }
        public PricingService(CarsContext carsContext, OrderLineUsedService _usedService)
        {
            db = carsContext;
            usedService = _usedService;
        }

        public PagingViewModel<OrderDetails> getOrders(int currentPage)
        {
            var orders = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 1 && !c.Price.HasValue).Include(c => c.Order.Vehicle).Include("OrderDetailsType").Skip((currentPage - 1) * TablesMaxRows.IndexPricingMaxRows).Take(TablesMaxRows.IndexPricingMaxRows).OrderByDescending(c=>c.DTsCreate).ToList();

            PagingViewModel<OrderDetails> viewModel = new PagingViewModel<OrderDetails>();         
            viewModel.items = orders.ToList();
            var itemsCount = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 1 && !c.Price.HasValue).OrderByDescending(c => c.DTsCreate).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexPricingMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexPricingMaxRows;
            return viewModel;
        }

        internal PagingViewModel<OrderDetails> SearchOrderLines(string search)
        {
            var orders = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 1 && !c.Price.HasValue && c.Items.Trim().ToLower().Contains(search.Trim().ToLower()))
               .Include(c => c.Order.Vehicle).Include("OrderDetailsType").Include("OrderDetailsType").Take(100).OrderByDescending(c => c.DTsCreate).ToList();
            PagingViewModel<OrderDetails> viewModel = new PagingViewModel<OrderDetails>();
            viewModel.items = orders.ToList();
            var itemsCount = orders.Count;
            double pageCount = 1;
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = 1;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = 100;
            return viewModel;
        }
        public PagingViewModel<OrderDetails> getOrdersWithChangelength(int currentPageIndex, int length)
        {
            TablesMaxRows.IndexPricingMaxRows = length;
            return getOrders(currentPageIndex);
        }
        //readonly object _object = new object();
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
            var VendorLocations = db.Branches.ToList();
            if (VendorLocations.Count() > 0)
            {
                return new SelectList(VendorLocations, "BranchID", "Name");
            }
            return null;
        }
        internal List<OrderDetails> GetOrderDeatilsChildren(long parentID)
        {
            var Children= db.OrderDetails.Include(c => c.UserBranch.Branch).Include("OrderDetailsType").Include("VendorLocation").Where(c => c.ParentOrderDetailsID.HasValue && c.ParentOrderDetailsID.Value == parentID).OrderByDescending(c=>c.DTsCreate).ToList();
            return Children;
        }

        internal int AddPricingField(long orderDetailsID, string partNumber, decimal price, int vendorLocationID, string comments ,string user)
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
                db.SaveChanges();

                if (orderDetails.Order.WithMaintenance.HasValue && orderDetails.Order.WithMaintenance.Value)
                {
                    //orderDetails.WorkflowID = 3;
                    usedService.ChangeWorkflow(orderDetails.OrderDetailsID, user);
                    usedService.ChangeDTsWorkflowEnter(orderDetails.OrderDetailsID);
                }
                else
                {
                    orderDetails.WorkflowID = 4;
                    WorkflowOrderDetailsLog log = new WorkflowOrderDetailsLog()
                    {
                        WorkflowID = 1,
                        Active = true,
                        DTsCreate = DateTime.Now,
                        OrderDetailsID = orderDetailsID,
                        SystemUserID = user,
                        Details = "From Pricing Team"
                    };
                    db.WorkflowOrderDetailsLogs.Add(log);
                    db.SaveChanges();
                }             
                return 1;
            }
            catch (Exception)
            {

                return -1;
            }
        
        }
        internal OrderDetails GetOrderDetailsByOrderDetailsID(long orderDetailsID)
        {
            var orderDetails = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 1 && c.OrderDetailsID == orderDetailsID).Include("OrderDetailsType").FirstOrDefault();
            orderDetails.Children = db.OrderDetails.Where(c=> c.ParentOrderDetailsID.HasValue && c.ParentOrderDetailsID == orderDetails.OrderDetailsID).OrderByDescending(c=>c.DTsCreate).ToList();
            return orderDetails;
        }

        internal OrderDetails AddOrderLine(PricingAddOrderLineViewModel model,string user)
        {
            try {
                var parent = db.OrderDetails.Include(c => c.UserBranch.Branch).Include("Order").FirstOrDefault(c => c.OrderDetailsID == model.ParentOrderDetailsId);
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
                    SystemUserCreate = user,
                    StatusID = 2,
                    UserBranch = parent.UserBranch,
                    Maintenance = parent.Order.WithMaintenance.HasValue ? parent.Order.WithMaintenance.Value : false

                };
                if (parent.Order.WithMaintenance.HasValue && parent.Order.WithMaintenance.Value)
                {
                    orderDetails.WorkflowID = 1;
                }
                else
                {
                    orderDetails.WorkflowID = 4;
                }
                db.OrderDetails.Add(orderDetails);
                db.SaveChanges();
                orderDetails.Prefix = $"{parent.UserBranch.Branch.BranchIP} : {parent.OrderID} : {orderDetails.OrderDetailsID}";
                db.SaveChanges();
                OrderDetailsStatusLog statusLog = new OrderDetailsStatusLog()
                {
                    DTsCreate = DateTime.Now,
                    OrderDetailsID = orderDetails.OrderDetailsID,
                    StatusID = 2,
                    SystemUserID = user
                };
                db.OrderDetailsStatusLogs.Add(statusLog);
                WorkflowOrderDetailsLog workflowLog = new WorkflowOrderDetailsLog()
                {
                    DTsCreate =DateTime.Now,
                    SystemUserID= user,
                    OrderDetailsID = orderDetails.OrderDetailsID,
                    WorkflowID = 1,
                    Active=true,
                    Details= "From Pricing Team"
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
            var OrderDetailsTypes = db.OrderDetailsType.ToList();
            if (OrderDetailsTypes.Count() > 0)
            {
                return new SelectList(OrderDetailsTypes, "OrderDetailsTypeID", "NameEn");
            }
            return null;
        }
        internal List<OrderDetails> GetReturnedOrderLine()
        {
            var orderDetails = db.OrderDetails.Where(c => (c.StatusID == 10 || c.StatusID == 9 ) && c.WorkflowID == 1 ).Include(c => c.OrderDetailsType).Include(c => c.Order.Vehicle.Brand).Include(c => c.Order.Vehicle.BrandModel).OrderBy(c => c.DTsCreate).ToList();
            return orderDetails;
        }
        internal OrderDetails EditOrderDetailsReturned(long orderDetailsID, string partNumber, decimal price, int vendorLocationID, string comments, string UserId)
        {
            OrderDetails orderDetails = db.OrderDetails.FirstOrDefault(c => c.OrderDetailsID == orderDetailsID &&(c.StatusID == 9 || c.StatusID == 10)  && c.WorkflowID == 1 );
            orderDetails.PartNumber = partNumber.Trim();
            orderDetails.Price = price;
            orderDetails.VendorLocationID = vendorLocationID;
            orderDetails.Comments = comments;
            orderDetails.UsedByUser = null;
            orderDetails.UsedDateTime = null;
            orderDetails.UsedByUser2 = null;
            orderDetails.UsedDateTime2 = null;
            OrderDetailsStatusLog statusLog = new OrderDetailsStatusLog()
            {
                DTsCreate = DateTime.Now,
                OrderDetailsID = orderDetails.OrderDetailsID,
                SystemUserID = UserId,
                WorkflowID = 1
            };
            if (orderDetails.StatusID == 9)
            {
                orderDetails.StatusID = 11;
                statusLog.StatusID = 11;
                statusLog.Detatils = "change status from 9 (ReversPricingAndLabor) to 11 (ReversLabor) by Pricing team after edit order details";
            }
            else if (orderDetails.StatusID == 10)
            {
                orderDetails.StatusID = 2;
                statusLog.StatusID = 2;
                statusLog.Detatils = "change status from 10 (ReversPricing) to 2 (WIP) by Pricing team after edit order details";
                orderDetails.WorkflowID = 4;
                WorkflowOrderDetailsLog workflowOrder = new WorkflowOrderDetailsLog()
                {
                    DTsCreate = DateTime.Now,
                    OrderDetailsID = orderDetails.OrderDetailsID,
                    SystemUserID = UserId,
                    WorkflowID = 4,
                    Active = true,
                };
                db.Add(workflowOrder);
            }
            db.OrderDetailsStatusLogs.Add(statusLog);
            db.SaveChanges();
            return orderDetails;
        }
    }
}
