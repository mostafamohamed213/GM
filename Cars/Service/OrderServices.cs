using Cars.Models;
using Cars.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Cars.Consts;

namespace Cars.Service
{
    public class OrderServices
    {
        public CarsContext db { get; set; }
        public OrderServices(CarsContext carsContext)
        {
            db = carsContext;     
        }

        public PagingViewModel<Order> getOrdersWithChangelength(int currentPageIndex, int length)
        {
            TablesMaxRows.IndexOrdersMaxRows = length;
            return getOrders(currentPageIndex);
        }
        public PagingViewModel<OrderDetails> getOrderLinesWithChangelength(int currentPageIndex, int length)
        {
            TablesMaxRows.IndexOrderLinesMaxRows = length;
            return getOrderLines(currentPageIndex);
        }
        public PagingViewModel<Order> getOrdersDraftWithChangelength(int currentPageIndex, int length)
        {
            TablesMaxRows.IndexOrdersDraftMaxRows = length;
            return getOrdersDraft(currentPageIndex);
        }
        public PagingViewModel<Order> getOrders(int currentPage)
        {
           var orders= db.Orders.Where(c => c.StatusID != 1 && c.StatusID != 5).
                Include("Vehicle").Include("Customer").Include("Customer.CustomerContacts").Skip((currentPage - 1) * TablesMaxRows.IndexOrdersMaxRows).Take(TablesMaxRows.IndexOrdersMaxRows).ToList();

            PagingViewModel<Order> viewModel = new PagingViewModel<Order> ();
            //var Brands = unitOfWork.Brands.
            //   FindAll(null, (currentPage - 1) * TablesMaxRows.InventoryBrandIndex, TablesMaxRows.InventoryBrandIndex, d => d.Name, OrderBy.Ascending);
            viewModel.items = orders.ToList();
            var itemsCount = db.Orders.Where(c => c.StatusID != 1 && c.StatusID != 5).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexOrdersMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexOrdersMaxRows;
            return viewModel;
        }

        internal PagingViewModel<Order> SearchOrderHeader(string search)
        {
            var orders = db.Orders.Where(c => c.StatusID != 1 && c.StatusID != 5 && (c.Vehicle.Name.Trim().ToLower().Contains(search.Trim().ToLower())|| c.Vehicle.Chases.Trim().ToLower().Contains(search.Trim().ToLower()))).
               Include("Vehicle").Include("Customer").Include("Customer.CustomerContacts").Take(100).ToList();
            PagingViewModel<Order> viewModel = new PagingViewModel<Order>();           
            viewModel.items = orders.ToList();
            var itemsCount = orders.Count;
            double pageCount = 1;
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = 1;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = 100;
            return viewModel;
        }

        public PagingViewModel<OrderDetails> getOrderLines(int currentPage)
        {
            var orders = db.OrderDetails.Where(c=> c.StatusID != 1 && c.StatusID != 5 && (c.WorkflowID == 1 || c.WorkflowID == 2)).Include("OrderDetailsType").Skip((currentPage - 1) * TablesMaxRows.IndexOrderLinesMaxRows).Take(TablesMaxRows.IndexOrderLinesMaxRows).ToList();

            PagingViewModel<OrderDetails> viewModel = new PagingViewModel<OrderDetails>();
            //var Brands = unitOfWork.Brands.
            //   FindAll(null, (currentPage - 1) * TablesMaxRows.InventoryBrandIndex, TablesMaxRows.InventoryBrandIndex, d => d.Name, OrderBy.Ascending);
            viewModel.items = orders.ToList();
            var itemsCount = db.OrderDetails.Where(c => c.StatusID != 1 && c.StatusID != 5 && (c.WorkflowID == 1 || c.WorkflowID == 2)).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexOrderLinesMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexOrderLinesMaxRows;
            return viewModel;
        }
        public PagingViewModel<Order> getOrdersDraft(int currentPage)
        {
            var orders = db.Orders.Include("Vehicle").Include("Customer").Include("Customer.CustomerContacts").Where(c=> c.StatusID == 1).Skip((currentPage - 1) * TablesMaxRows.IndexOrdersDraftMaxRows).Take(TablesMaxRows.IndexOrdersDraftMaxRows).ToList();

            PagingViewModel<Order> viewModel = new PagingViewModel<Order>();
            //var Brands = unitOfWork.Brands.
            //   FindAll(null, (currentPage - 1) * TablesMaxRows.InventoryBrandIndex, TablesMaxRows.InventoryBrandIndex, d => d.Name, OrderBy.Ascending);
            viewModel.items = orders.ToList();
            var itemsCount = db.Orders.Where(c => c.StatusID == 1).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexOrdersDraftMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexOrdersDraftMaxRows;
            return viewModel;
        }
        object b = new object();
        internal OrderDetails GetOrderDetailsByOrderDetailsID(long orderDetailsID ,string user)
        {
            return db.OrderDetails.Where(c => c.StatusID != 5 && (c.WorkflowID == 1 || c.WorkflowID == 2) && c.OrderDetailsID == orderDetailsID).Include("Order").Include("Order.Vehicle").Include("Order.Customer").Include("Order.Customer.CustomerContacts").FirstOrDefault();
        }

        public Order GetOrderByID(long orderId)
        {
            try
            {
                Order order = db.Orders.Where(c => c.OrderID == orderId).Include("Vehicle").Include("Customer")
                                .Include("Customer.CustomerContacts").Include(c=>c.OrderDetails.Where(x=> x.StatusID != 5)).Include("OrderDetails.OrderDetailsType").FirstOrDefault();
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

        //internal int SaveOrderAsDraft(OrderViewModel orderModel)
        //{
        //    try
        //    {
        //        DraftOrder draftOrder = new DraftOrder()
        //        {
        //            DTsCreate = DateTime.Now,
        //            SystemUserCreate = "-1",
        //            Brand = string.IsNullOrEmpty(orderModel.Brand) ? null : orderModel.Brand,
        //            Chases = string.IsNullOrEmpty(orderModel.Chases) ? null : orderModel.Chases,
        //            EmployeeBranchID = 10,
        //            Model = string.IsNullOrEmpty(orderModel.Model) ? null : orderModel.Model,
        //            Name = string.IsNullOrEmpty(orderModel.VehicleName) ? null : orderModel.VehicleName,
        //            Year = string.IsNullOrEmpty(orderModel.Year) ? null : orderModel.Year,
        //            Phone = string.IsNullOrEmpty(orderModel.CustomerPhone) ? null : orderModel.CustomerPhone,
        //            WithMaintenance = orderModel.WithMaintenance,
        //            Enable = true
        //        };
        //        db.DraftOrders.Add(draftOrder);
        //        db.SaveChanges();
        //        return 1;
        //    }
        //    catch (Exception)
        //    {
        //        return -1;
        //    }           
        //}

        internal int SaveAsDraft(long orderId)
        {

            Order order = db.Orders.Include("OrderDetails").FirstOrDefault(c => c.OrderID == orderId);
            Workflow _Workflow = db.Workflows.FirstOrDefault(c => c.WorkflowID == 1);
            if (order is not null)
            {
                order.StatusID = 1;
                if (order.OrderDetails.Count > 0)
                {
                    foreach (var item in order.OrderDetails)
                    {
                        item.StatusID = 1;
                        item.WorkflowID = _Workflow.WorkflowID;
                        item.Workflow = _Workflow;
                    }
                }
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        internal int SaveOrder(long orderId,string user)
        {
            Order order = db.Orders.Include("OrderDetails").FirstOrDefault(c => c.OrderID == orderId);
            Workflow _Workflow = db.Workflows.FirstOrDefault(c => c.WorkflowID == 2);
            if (order is not null)
            {
                order.StatusID = 2;
                if (order.OrderDetails.Count > 0)
                {
                    foreach (var item in order.OrderDetails)
                    {
                        item.StatusID = 2;
                        item.WorkflowID = _Workflow.WorkflowID;
                        item.Workflow = _Workflow;

                        WorkflowOrderDetailsLog workflowOrder = new WorkflowOrderDetailsLog()
                        {
                            DTsCreate = DateTime.Now,
                            OrderDetailsID = item.OrderDetailsID,
                            SystemUserID = user,
                            WorkflowID = 1,
                            Active =true,                            
                        };
                        db.Add(workflowOrder);
                        OrderDetailsStatusLog statusLog = new OrderDetailsStatusLog()
                        {
                            DTsCreate = DateTime.Now,
                            OrderDetailsID = item.OrderDetailsID,
                            SystemUserID = user,
                            StatusID = 2,                           
                        };
                        db.Add(statusLog);
                    }
                }
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        internal DraftOrder getOrderDraftById(long orderDraftId)
        {
            return db.DraftOrders.FirstOrDefault(C => C.DraftOrderID == orderDraftId && C.Enable);
        }

        public long AddOrder(OrderViewModel model,string user)
        {
            try
            {
                if (model is not null)
                {
                    DateTime now = DateTime.Now;
                    Vehicle vehicle = new Vehicle()
                    {
                        DTsCreate = now,
                        SystemUserCreate = user,
                        Chases = model.Chases.Trim(),
                        Name = model.VehicleName.Trim(),
                        Year = model.Year?.Trim(),
                        Model = model.Model?.Trim(),
                        Brand = model.Brand?.Trim(),
                    };
                    Customer customer = new Customer()
                    {
                        DTsCreate = now,
                        SystemUserCreate = user
                    };
                    CustomerContact contact = new CustomerContact()
                    {
                        DTsCreate = now,
                        SystemUserCreate = user,
                        CustomerID = customer.CustomerID,
                        Customer =customer,
                        Phone = model.CustomerPhone,
                        HasTelegram = false,
                        HasWhatsapp = false
                    };
                    var UserBranch = db.UserBranches.FirstOrDefault(c => c.IsActive && c.UserID == user);
                    Order order = new Order()
                    {
                        DTsCreate = now,
                        SystemUserCreate = user,
                        Customer = customer,
                        UserBranchID = UserBranch != null ? UserBranch.UserBranchID : -1,
                        Vehicle = vehicle,
                        WithMaintenance = model.WithMaintenance,
                        StatusID =1

                    };
                    db.Orders.Add(order);
                    db.SaveChanges();
                    db.CustomerContacts.Add(contact);
                    db.SaveChanges();

                    //if (model.DraftId.HasValue && model.DraftId.Value>0)
                    //{
                    //  DraftOrder draft=  getOrderDraftById(model.DraftId.Value);
                    //    draft.Enable = false;
                    //    db.SaveChanges();
                    //}
                    return order.OrderID;
                }
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
           
        }

        internal long DeleteOrderDetails(long orderDetailsID,string User)
        {
            try
            {
                OrderDetails orderDetails = GetOrderDetailsByOrderDetailsID(orderDetailsID, User);
                orderDetails.StatusID = 5;
                OrderDetailsStatusLog statusLog = new OrderDetailsStatusLog()
                {
                    DTsCreate = DateTime.Now,
                    OrderDetailsID = orderDetails.OrderDetailsID,
                    SystemUserID = User,
                    StatusID = 5,                    
                };
                db.Add(statusLog);
                db.SaveChanges();
                OpenOrderDetails(orderDetails.OrderDetailsID);
                return orderDetails.OrderID;
            }
            catch (Exception)
            {

                return -1;
            }
          
        }
        internal long CancelOrderDetails(long orderDetailsID ,string user)
        {
            try
            {
                OrderDetails orderDetails = GetOrderDetailsByOrderDetailsID(orderDetailsID, user);
                orderDetails.StatusID = 4;
                OrderDetailsStatusLog statusLog = new OrderDetailsStatusLog()
                {
                    DTsCreate = DateTime.Now,
                    OrderDetailsID = orderDetails.OrderDetailsID,
                    SystemUserID = user,
                    StatusID = 4,
                };
                db.Add(statusLog);
                db.SaveChanges();
                OpenOrderDetails(orderDetails.OrderDetailsID);
                return orderDetails.OrderID;
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

        internal long EditOrderDetailsFromSales(string items, int quantity, int type, bool approved, long orderDetailsID)
        {
            try
            {
                OrderDetails orderDetails = db.OrderDetails.FirstOrDefault(c => c.OrderDetailsID == orderDetailsID);
                if (orderDetails is null)
                {
                    return 0;
                }
                orderDetails.Items = items.Trim();
                orderDetails.Quantity = quantity;
                orderDetails.OrderDetailsTypeID = type;
                orderDetails.IsApproved = approved;                
                db.SaveChanges();
                return orderDetails.OrderID;
            }
            catch (Exception)
            {
                return -1;
            }        
        }
       

        internal List<OrderDetailsViewModel> GetOrderDetailsByOrderId(long orderid)
        {
            var List = db.OrderDetails.Where(c => c.OrderID == orderid && c.StatusID != 5).Include("OrderDetailsType").ToList();
            if (List.Count > 0)
            {
                List<OrderDetailsViewModel> model = new List<OrderDetailsViewModel>();
                foreach (var item in List)
                {
                    //model.Add(new OrderDetailsViewModel() { Enabled = item.Enabled.HasValue ? item.Enabled.Value : false, OrderID = item.OrderID, IsApproved = item.IsApproved, Items = item.Items, OrderDetailsID = item.OrderDetailsID, Quantity = item.Quantity, type = item.OrderDetailsType.NameEn, Price = item.Price, PartNumber = item.PartNumber, BranchID = item.BranchID, Comments = item.Comments, CanceledByUserID = item.c });
                    model.Add(new OrderDetailsViewModel() {OrderID = item.OrderID, IsApproved = item.IsApproved, Items = item.Items, OrderDetailsID = item.OrderDetailsID, Quantity = item.Quantity, type = item.OrderDetailsType.NameEn, Price = item.Price, PartNumber = item.PartNumber, Comments = item.Comments});

                }
                return model;
            }
            return null;
        }

        internal int AddOrderDetails(string items, int quantity, int type, bool approved,long orderID,string user)
        {
          
            try
            {
                var order = db.Orders.FirstOrDefault(c => c.OrderID == orderID);
                var UserBranch = db.UserBranches.FirstOrDefault(c => c.IsActive && c.UserID == user);
                OrderDetails orderDetails = new OrderDetails()
                {
                    DTsCreate = DateTime.Now,
                    SystemUserCreate = user,
                    Items = items,
                    Quantity = quantity,
                    OrderDetailsTypeID = type,
                    IsApproved = approved ? approved : null,
                    OrderID = orderID,
                    UserBranchID = UserBranch != null ? UserBranch.UserBranchID : -1,
                    StatusID = order.StatusID,
                    WorkflowID = order.StatusID == 2 ? 2 : 1
                };
                db.OrderDetails.Add(orderDetails);
                db.SaveChanges();
                if (order.StatusID == 2)
                {
                    WorkflowOrderDetailsLog workflowOrder = new WorkflowOrderDetailsLog()
                    {
                        DTsCreate = DateTime.Now,
                        OrderDetailsID = orderDetails.OrderDetailsID,
                        SystemUserID = user,
                        WorkflowID = 1,
                        Active = true,
                    };
                    db.Add(workflowOrder);
                    OrderDetailsStatusLog statusLog = new OrderDetailsStatusLog()
                    {
                        DTsCreate = DateTime.Now,
                        OrderDetailsID = orderDetails.OrderDetailsID,
                        SystemUserID = user,
                        StatusID = 2,                       
                    };
                    db.Add(statusLog);
                    db.SaveChanges();
                }               
                return 1;
            }
            catch (Exception)
            {

                return -1;
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
    }
}
