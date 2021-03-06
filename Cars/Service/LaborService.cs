using Cars.Consts;
using Cars.Models;
using Cars.ViewModels;
using Cars.Service;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service
{
    public class LaborService
    {
        public CarsContext db { get; set; }
        public OrderLineUsedService usedService { get; set; }
        public LaborService(CarsContext carsContext, OrderLineUsedService _usedService)
        {
            db = carsContext;
            usedService = _usedService;
        }

        /*public PagingViewModel<Order> getOrdersWithChangelength(int currentPageIndex, int length)
        {
            TablesMaxRows.IndexLaborMaxRows = length;
            return getOrders(currentPageIndex);
        }*/
        public PagingViewModel<OrderDetails> getOrderLinesWithChangelength(int currentPageIndex, int length)
        {
            TablesMaxRows.IndexLaborMaxRows = length;
            return getOrderLines(currentPageIndex);
        }
        public PagingViewModel<OrderDetails> getOrderLines(int currentPage)
        {

            var orders = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 1 && c.Order.WithMaintenance.HasValue && c.Order.WithMaintenance.Value).Include(c => c.Order.Vehicle).Include(c => c.VendorLocation).Include("OrderDetailsType").Skip((currentPage - 1) * TablesMaxRows.IndexLaborMaxRows).Take(TablesMaxRows.IndexLaborMaxRows).OrderByDescending(c => c.DTsCreate).ToList();

            PagingViewModel<OrderDetails> viewModel = new PagingViewModel<OrderDetails>();
            viewModel.items = orders.ToList();
            var itemsCount = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 1 && c.Order.WithMaintenance.HasValue && c.Order.WithMaintenance.Value).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexLaborMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexLaborMaxRows;
            return viewModel;
        }

        public PagingViewModel<OrderDetails> getByType(int currentPage, string? type, decimal? from, decimal? to, int? vendor)
        {
            var orders = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 1 && c.Order.WithMaintenance.HasValue && c.Order.WithMaintenance.Value).Include(c => c.Order.Vehicle).Include("OrderDetailsType").Skip((currentPage - 1) * TablesMaxRows.IndexLaborMaxRows).Take(TablesMaxRows.IndexLaborMaxRows).OrderByDescending(c => c.DTsCreate).ToList();

            if (type != "all" && type != null)
            {
                orders = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 1 && c.Order.WithMaintenance.HasValue && c.Order.WithMaintenance.Value && c.OrderDetailsType.NameEn == type).Include(c => c.Order.Vehicle).Include("OrderDetailsType").Skip((currentPage - 1) * TablesMaxRows.IndexLaborMaxRows).Take(TablesMaxRows.IndexLaborMaxRows).OrderByDescending(c => c.DTsCreate).ToList();
            }

            if (from != null || to != null || vendor != null)
            {
                if (from != null)
                {
                    orders = orders.Where(c => c.Price >= from).ToList();
                }
                if (to != null)
                {
                    orders = orders.Where(c => c.Price <= to).ToList();
                }
                if (vendor != null)
                {
                    orders = orders.Where(c => c.VendorLocationID == vendor).ToList();
                }
            }


            PagingViewModel<OrderDetails> viewModel = new PagingViewModel<OrderDetails>();
            viewModel.items = orders.ToList();
            var itemsCount = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 1 && c.Order.WithMaintenance.HasValue && c.Order.WithMaintenance.Value).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexLaborMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexLaborMaxRows;
            return viewModel;
        }

        /*public PagingViewModel<Order> getOrders(int currentPage)
        {
            var orders = db.Orders.Include("Vehicle").Include("Customer").Include("Customer.CustomerContacts").Where(c => c.StatusID == 2).Skip((currentPage - 1) * TablesMaxRows.IndexLaborMaxRows).Take(TablesMaxRows.IndexLaborMaxRows).ToList();

            PagingViewModel<Order> viewModel = new PagingViewModel<Order>();
            //var Brands = unitOfWork.Brands.
            //   FindAll(null, (currentPage - 1) * TablesMaxRows.InventoryBrandIndex, TablesMaxRows.InventoryBrandIndex, d => d.Name, OrderBy.Ascending);
            viewModel.items = orders.ToList();
            var itemsCount = db.Orders.Where(c => c.StatusID == 2).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexOrdersMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexOrdersMaxRows;
            return viewModel;
        }*/

        public Order GetOrderByID(long orderId)
        {
            try
            {
                Order order = db.Orders.Where(c => c.OrderID == orderId).Include("Vehicle").Include("Customer")
                                .Include("Customer.CustomerContacts").Include(c => c.OrderDetails.Where(x => x.StatusID != 5)).Include("OrderDetails.OrderDetailsType").FirstOrDefault();
                if (order is not null)
                {
                    return order;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }

        internal List<OrderDetails> GetReturnedOrderLine()
        {
            var orderDetails = db.OrderDetails.Where(c => c.StatusID == 11 && c.WorkflowID == 1).Include(c => c.OrderDetailsType).Include(c => c.Order.Vehicle.Brand).Include(c => c.Order.Vehicle.BrandModel).OrderBy(c => c.DTsCreate).ToList();
            return orderDetails;
        }

        internal OrderDetails GetOrderDetailsByOrderDetailsID(long orderDetailsID)
        {
            var orderDetails = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 1).Include("Order").Include("Order.Vehicle").Include("Order.Customer").Include("Order.Customer.CustomerContacts").FirstOrDefault(c => c.OrderDetailsID == orderDetailsID);
            return orderDetails;
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

        internal OrderDetails EditOrderDetailsReturned(long orderDetailsID, decimal labor_Hours, double labor_Value, string UserId)
        {
            OrderDetails orderDetails = db.OrderDetails.FirstOrDefault(c => c.OrderDetailsID == orderDetailsID && c.StatusID == 11 && c.WorkflowID == 1);
            orderDetails.Labor_Hours = labor_Hours;
            orderDetails.Labor_Value = labor_Value;
            orderDetails.UsedByUser = null;
            orderDetails.UsedDateTime = null;
            orderDetails.UsedByUser2 = null;
            orderDetails.UsedDateTime2 = null;
            orderDetails.StatusID = 2;
            OrderDetailsStatusLog statusLog = new OrderDetailsStatusLog()
            {
                DTsCreate = DateTime.Now,
                OrderDetailsID = orderDetails.OrderDetailsID,
                SystemUserID = UserId,
                StatusID = 2,
                WorkflowID = 1,
                Detatils = "change status from 11 (ReversLabor) to 2 (WIP) by Labor team after edit order details"
            };
            db.OrderDetailsStatusLogs.Add(statusLog);
            orderDetails.WorkflowID = 4;
            WorkflowOrderDetailsLog workflowOrder = new WorkflowOrderDetailsLog()
            {
                DTsCreate = DateTime.Now,
                OrderDetailsID = orderDetails.OrderDetailsID,
                SystemUserID = UserId,
                WorkflowID = 1,
                Active = true,
                Details = "from labor team"
            };
            db.WorkflowOrderDetailsLogs.Add(workflowOrder);
            db.SaveChanges();
            return orderDetails;
        }

        public int BulkEditOrderDetailsReturned(List<OrderDetails> model, string UserId)
        {
            var orderDetailsIDs = model.Select(x => x.OrderDetailsID);
            var ordersDetails = db.OrderDetails.Where(c => orderDetailsIDs.Contains(c.OrderDetailsID) && c.StatusID == 11 && c.WorkflowID == 1);
            List<OrderDetailsStatusLog> statusLogs = new List<OrderDetailsStatusLog>();
            List<WorkflowOrderDetailsLog> workflowOrderLogs = new List<WorkflowOrderDetailsLog>();
            foreach (var entity in ordersDetails)
            {
                var item = model.First(x => x.OrderDetailsID == entity.OrderDetailsID);

                entity.Labor_Hours = item.Labor_Hours;
                entity.Labor_Value = item.Labor_Value;
                entity.UsedByUser = null;
                entity.UsedDateTime = null;
                entity.UsedByUser2 = null;
                entity.UsedDateTime2 = null;
                entity.StatusID = 2;
                entity.WorkflowID = 4;

                OrderDetailsStatusLog statusLog = new OrderDetailsStatusLog()
                {
                    DTsCreate = DateTime.Now,
                    OrderDetailsID = entity.OrderDetailsID,
                    SystemUserID = UserId,
                    StatusID = 2,
                    WorkflowID = 1,
                    Detatils = "change status from 11 (ReversLabor) to 2 (WIP) by Labor team after edit order details"
                };
                statusLogs.Add(statusLog);
                WorkflowOrderDetailsLog workflowOrder = new WorkflowOrderDetailsLog()
                {
                    DTsCreate = DateTime.Now,
                    OrderDetailsID = entity.OrderDetailsID,
                    SystemUserID = UserId,
                    WorkflowID = 1,
                    Active = true,
                    Details = "from labor team"
                };
                workflowOrderLogs.Add(workflowOrder);
            }
            db.WorkflowOrderDetailsLogs.AddRange(workflowOrderLogs);
            db.OrderDetailsStatusLogs.AddRange(statusLogs);
            return db.SaveChanges();
        }

        internal long EditOrderDetailsFromSales(string items, int quantity, int type, bool approved, decimal? labor_hours, double? labor_value, long orderDetailsID, string UserId)
        {
            try
            {
                OrderDetails orderDetails = db.OrderDetails.FirstOrDefault(c => c.OrderDetailsID == orderDetailsID);
                if (orderDetails is null)
                {
                    return 0;
                }

                //orderDetails.Items = items.Trim();
                //orderDetails.Quantity = quantity;
                //orderDetails.OrderDetailsTypeID = type;
                //orderDetails.IsApproved = approved;         
                if (labor_hours != null && labor_value != null)
                {
                    orderDetails.Labor_Hours = labor_hours;
                    orderDetails.Labor_Value = labor_value;
                    db.SaveChanges();
                    usedService.ChangeWorkflow(orderDetails.OrderDetailsID, UserId);
                    usedService.ChangeDTsWorkflowEnter(orderDetails.OrderDetailsID);
                    return orderDetails.OrderID;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        internal long OpenOrderDetails(long orderDetailsID)
        {
            try
            {
                var orderDetails = db.OrderDetails.FirstOrDefault(c => c.OrderDetailsID == orderDetailsID);
                orderDetails.UsedByUser2 = null;
                orderDetails.UsedDateTime2 = null;
                db.SaveChanges();
                return orderDetails.OrderDetailsID;
            }
            catch (Exception)
            {

                return 0;
            }
        }

        private object _object = new object();
        public int BulkEditAndOpenAndOrdersDetails(List<OrderDetails> ordersDetails, string userID)
        {
            try
            {
                lock (_object)
                {
                    var ids = ordersDetails.Select(x => x.OrderDetailsID);
                    var userRoles = db.UserRoles.Where(c => c.UserId == userID).FirstOrDefault();
                    var ordersDetailsEntites = db.OrderDetails.Where(c => ids.Contains(c.OrderDetailsID));

                    if (userRoles is null || ordersDetailsEntites is null || ordersDetailsEntites.Count() <= 0)
                        return 0;

                    List<WorkflowOrderDetailsLog> logs = new List<WorkflowOrderDetailsLog>();
                    //Open Order Details, Step to next workflow, 
                    foreach (var entity in ordersDetailsEntites)
                    {
                        var item = ordersDetails.First(x => x.OrderDetailsID == entity.OrderDetailsID);

                        //Change Workflow Enter 
                        if (entity.Price.HasValue && item.Labor_Hours.HasValue && item.Labor_Value.HasValue)
                            entity.DTsWorflowEnter = DateTime.Now;

                        //Add Labor Values 
                        if (item.Labor_Hours.HasValue && item.Labor_Value.HasValue)
                        {
                            entity.Labor_Hours = item.Labor_Hours;
                            entity.Labor_Value = item.Labor_Value;
                        }

                        //Open order Details 
                        if (userRoles.RoleId.Trim() == "04e215eb-d6d9-45bd-9d61-e04a81bfb04b".Trim()) //Sales
                        {
                            entity.UsedByUser = null;
                            entity.UsedDateTime = null;
                            entity.UsedByUser2 = null;
                            entity.UsedDateTime2 = null;
                        }
                        else if (userRoles.RoleId.Trim() == "29f2bbf4-3fa6-447a-8d5e-e16e2510d31b".Trim())//Pricing
                        {
                            entity.UsedByUser = null;
                            entity.UsedDateTime = null;
                        }
                        else if (userRoles.RoleId.Trim() == "9c9a27b5-1686-4714-9242-13ffa884fab2".Trim())//Labor
                        {
                            entity.UsedByUser2 = null;
                            entity.UsedDateTime2 = null;
                        }

                        //Change Workflow, Log
                        if (entity.Price.HasValue && !string.IsNullOrEmpty(entity.PartNumber) && item.Labor_Hours.HasValue && item.Labor_Value.HasValue)
                        {
                            entity.WorkflowID = 4;
                            WorkflowOrderDetailsLog log = new WorkflowOrderDetailsLog()
                            {
                                WorkflowID = 1,
                                Active = true,
                                DTsCreate = DateTime.Now,
                                OrderDetailsID = entity.OrderDetailsID,
                                SystemUserID = userID,
                                Details = userRoles.RoleId.Trim() == "29f2bbf4-3fa6-447a-8d5e-e16e2510d31b".Trim() ? "From Pricing Team" : "From Labor Team"
                            };
                            logs.Add(log);
                        }
                        else
                        {
                            WorkflowOrderDetailsLog log = new WorkflowOrderDetailsLog()
                            {
                                WorkflowID = 1,
                                Active = true,
                                DTsCreate = DateTime.Now,
                                OrderDetailsID = entity.OrderDetailsID,
                                SystemUserID = userID,
                                Details = userRoles.RoleId.Trim() == "29f2bbf4-3fa6-447a-8d5e-e16e2510d31b".Trim() ? "From Pricing Team" : "From Labor Team"
                            };
                            logs.Add(log);
                        }
                    }

                    db.WorkflowOrderDetailsLogs.AddRange(logs);
                    return db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
