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

        public PagingViewModel<Order> getOrdersWithChangelength(int currentPageIndex, int length ,string userId)
        {
            TablesMaxRows.IndexOrdersMaxRows = length;
            return getOrders(currentPageIndex, userId);
        }
        public PagingViewModel<OrderDetails> getOrderLinesWithChangelength(int currentPageIndex, int length,string userId)
        {
            TablesMaxRows.IndexOrderLinesMaxRows = length;
            return getOrderLines(currentPageIndex, userId);
        }
        public PagingViewModel<Order> getOrdersDraftWithChangelength(int currentPageIndex, int length,string userid)
        {
            TablesMaxRows.IndexOrdersDraftMaxRows = length;
            return getOrdersDraft(currentPageIndex, userid);
        }
        public PagingViewModel<Order> getOrders(int currentPage,string userid)
        {
           var orders= db.Orders.Where(c => c.StatusID != 1 && c.StatusID != 5 && c.SystemUserCreate == userid).
                Include("Vehicle").Include(c=>c.UserBranch.Branch).Include("Customer").Include("Customer.CustomerContacts").Skip((currentPage - 1) * TablesMaxRows.IndexOrdersMaxRows).Take(TablesMaxRows.IndexOrdersMaxRows).OrderByDescending(c=>c.DTsCreate).ToList();

            PagingViewModel<Order> viewModel = new PagingViewModel<Order> ();
            //var Brands = unitOfWork.Brands.
            //   FindAll(null, (currentPage - 1) * TablesMaxRows.InventoryBrandIndex, TablesMaxRows.InventoryBrandIndex, d => d.Name, OrderBy.Ascending);
            viewModel.items = orders.ToList();
            var itemsCount = db.Orders.Where(c => c.StatusID != 1 && c.StatusID != 5 && c.SystemUserCreate == userid).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexOrdersMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexOrdersMaxRows;
            return viewModel;
        }

        internal PagingViewModel<Order> SearchOrderHeader(string search ,string userid)
        {
            var orders = db.Orders.Where(c => c.SystemUserCreate == userid &&c.StatusID != 1 && c.StatusID != 5 && (c.Vehicle.Name.Trim().ToLower().Contains(search.Trim().ToLower())|| c.Vehicle.Chases.Trim().ToLower().Contains(search.Trim().ToLower()))).
               Include("Vehicle").Include(c => c.UserBranch.Branch).Include("Customer").Include("Customer.CustomerContacts").Take(100).OrderByDescending(c => c.DTsCreate).ToList();
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

        internal PagingViewModel<OrderDetails> SearchOrderLines(string search,string userid)
        {
            var orders = db.OrderDetails.Where(c => c.Order.SystemUserCreate == userid && !c.InventoryID.HasValue && c.StatusID != 1 && c.StatusID != 5 && (c.WorkflowID == 1 || c.WorkflowID == 2) && c.Items.Trim().ToLower().Contains(search.Trim().ToLower())).
               Include("OrderDetailsType").Include(c => c.Order).Include(c => c.UserBranch.Branch).Take(100).OrderByDescending(c => c.DTsCreate).ToList();
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

        public PagingViewModel<OrderDetails> getOrderLines(int currentPage,string userId)
        {
            var orders = db.OrderDetails.Where(c=> c.Order.SystemUserCreate == userId && c.StatusID != 1 && c.StatusID != 5 && !c.InventoryID.HasValue &&  (c.WorkflowID == 1 || c.WorkflowID == 2)).Include(c => c.Order).Include(c => c.UserBranch.Branch).Include("OrderDetailsType").Skip((currentPage - 1) * TablesMaxRows.IndexOrderLinesMaxRows).Take(TablesMaxRows.IndexOrderLinesMaxRows).OrderByDescending(c => c.DTsCreate).ToList();

            PagingViewModel<OrderDetails> viewModel = new PagingViewModel<OrderDetails>();
            //var Brands = unitOfWork.Brands.
            //   FindAll(null, (currentPage - 1) * TablesMaxRows.InventoryBrandIndex, TablesMaxRows.InventoryBrandIndex, d => d.Name, OrderBy.Ascending);
            viewModel.items = orders.ToList();
            var itemsCount = db.OrderDetails.Where(c => c.Order.SystemUserCreate == userId &&  c.StatusID != 1 && c.StatusID != 5 && !c.InventoryID.HasValue && (c.WorkflowID == 1 || c.WorkflowID == 2)).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexOrderLinesMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexOrderLinesMaxRows;
            return viewModel;
        }
        public PagingViewModel<Order> getOrdersDraft(int currentPage,string userId)
        {
            var orders = db.Orders.Include("Vehicle").Include(c => c.UserBranch.Branch).Include(c => c.UserBranch).Include("Customer").Include("Customer.CustomerContacts").Where(c=> c.StatusID == 1 && c.SystemUserCreate == userId).Skip((currentPage - 1) * TablesMaxRows.IndexOrdersDraftMaxRows).Take(TablesMaxRows.IndexOrdersDraftMaxRows).ToList();

            PagingViewModel<Order> viewModel = new PagingViewModel<Order>();
            //var Brands = unitOfWork.Brands.
            //   FindAll(null, (currentPage - 1) * TablesMaxRows.InventoryBrandIndex, TablesMaxRows.InventoryBrandIndex, d => d.Name, OrderBy.Ascending);
            viewModel.items = orders.ToList();
            var itemsCount = db.Orders.Where(c => c.StatusID == 1 && c.SystemUserCreate == userId).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexOrdersDraftMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexOrdersDraftMaxRows;
            return viewModel;
        }

        internal PagingViewModel<Order> SearOrdersDraft(string search,string UserId)
        {
            var orders = db.Orders.Where(c => c.SystemUserCreate == UserId && c.StatusID ==1 && (c.Vehicle.Name.Trim().ToLower().Contains(search.Trim().ToLower()) || c.Vehicle.Chases.Trim().ToLower().Contains(search.Trim().ToLower()))).
               Include("Vehicle").Include(c => c.UserBranch).Include(c => c.UserBranch.Branch).Include("Customer").Include("Customer.CustomerContacts").Take(100).ToList();
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
        object b = new object();
        internal OrderDetails GetOrderDetailsByOrderDetailsID(long orderDetailsID ,string user)
        {
            return db.OrderDetails.Where(c => c.Order.SystemUserCreate == user &&  c.StatusID != 5 && (c.WorkflowID == 1 || c.WorkflowID == 2) && c.OrderDetailsID == orderDetailsID).Include(c=>c.UserBranch.Branch).Include("Order").Include(c => c.Order.UserBranch.Branch).Include("Order.Vehicle").Include("Order.Customer").Include("Order.Customer.CustomerContacts").FirstOrDefault();
        }

        public Order GetOrderByID(long orderId, string UserId)
        {
            try
            {
                Order order = db.Orders.Where(c => c.OrderID == orderId && c.SystemUserCreate == UserId).Include("Vehicle").Include("Customer")
                               .Include("UserBranch").Include("UserBranch.Branch").Include("Customer.CustomerContacts").Include(c=>c.OrderDetails.Where(x=> x.StatusID != 5).OrderByDescending(c=>c.DTsCreate)).Include("OrderDetails.OrderDetailsType").Include("OrderDetails.UserBranch.Branch").FirstOrDefault();
                //db.OrderDetails.Where(c => c.OrderID == order.OrderID && c.StatusID != 5).Include(c => c.UserBranch.Branch);
                if (order is not null)
                {
                    return order;
                }
                return null;
            }
            catch (Exception )
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
            Workflow _Workflow = db.Workflows.FirstOrDefault(c => c.WorkflowID == 1);
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
                        item.DTsWorflowEnter = DateTime.Now;

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
                OrderDetails orderDetails = db.OrderDetails.FirstOrDefault(c => c.OrderDetailsID == orderDetailsID && !c.InventoryID.HasValue);
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

        internal OrderDetails Reject(long orderDetailsID, string reason, string user)
        {
            var orderDetails = db.OrderDetails.Include(c=>c.Order).FirstOrDefault(c => c.OrderDetailsID == orderDetailsID && c.Order.SystemUserCreate ==user);
            if (orderDetails is not null)
            {
                orderDetails.StatusID = 6;
                OrderDetailsStatusLog statusLog = new OrderDetailsStatusLog()
                {
                    WorkflowID = orderDetails.WorkflowID,
                    StatusID = 6,
                    Reason = reason,
                    DTsCreate = DateTime.Now,
                    SystemUserID = user,
                    Detatils = "Reject From sales Team",
                    OrderDetailsID = orderDetails.OrderDetailsID,
                };
                db.OrderDetailsStatusLogs.Add(statusLog);
                db.SaveChanges();
            }
            return orderDetails;
        }

        internal OrderDetails Issued(long orderDetailsID, string user)
        {
            var orderDetails = db.OrderDetails.Include(c=>c.Order).FirstOrDefault(c => c.OrderDetailsID == orderDetailsID && c.Order.SystemUserCreate == user);
            if (orderDetails is not null)
            {
                orderDetails.StatusID = 3;
                OrderDetailsStatusLog statusLog = new OrderDetailsStatusLog()
                {
                    WorkflowID = orderDetails.WorkflowID,
                    StatusID = 3,                  
                    DTsCreate = DateTime.Now,
                    SystemUserID = user,
                    OrderDetailsID = orderDetails.OrderDetailsID,
                };
                db.OrderDetailsStatusLogs.Add(statusLog);
                db.SaveChanges();
            }
            return orderDetails;
        }

        internal List<OrderDetailsViewModel> GetOrderDetailsByOrderId(long orderid ,string userId)
        {
            var List = db.OrderDetails.Where(c => c.Order.SystemUserCreate == userId && c.OrderID == orderid && c.StatusID != 5).Include(c=>c.Order).Include(c => c.UserBranch.Branch).Include("OrderDetailsType").OrderByDescending(C=>C.DTsCreate).ToList();
            if (List.Count > 0)
            {
                List<OrderDetailsViewModel> model = new List<OrderDetailsViewModel>();
                foreach (var item in List)
                {
                    //model.Add(new OrderDetailsViewModel() { Enabled = item.Enabled.HasValue ? item.Enabled.Value : false, OrderID = item.OrderID, IsApproved = item.IsApproved, Items = item.Items, OrderDetailsID = item.OrderDetailsID, Quantity = item.Quantity, type = item.OrderDetailsType.NameEn, Price = item.Price, PartNumber = item.PartNumber, BranchID = item.BranchID, Comments = item.Comments, CanceledByUserID = item.c });
                    model.Add(new OrderDetailsViewModel() {workflowID=item.WorkflowID,statusID = item.StatusID,prefix=item.Prefix,OrderID = item.OrderID, IsApproved = item.IsApproved, Items = item.Items, OrderDetailsID = item.OrderDetailsID, Quantity = item.Quantity, type = item.OrderDetailsType.NameEn, Price = item.Price, PartNumber = item.PartNumber, Comments = item.Comments,deliveryID= string.IsNullOrEmpty(item.DeliveryID)?0:1,inventoryID=item.InventoryID.HasValue ? item.InventoryID.Value:0});

                }
                return model;
            }
            return null;
        }
        internal List<OrderDetails> GetOrderDetailsById(long orderid, string userId)
        {
            var List = db.OrderDetails.Where(c => c.Order.SystemUserCreate == userId && c.OrderID == orderid && c.StatusID != 5).Include(c=>c.Order).Include(c => c.UserBranch.Branch).Include("OrderDetailsType").OrderByDescending(C => C.DTsCreate).ToList();
            
            return List;
        }

        internal int AddOrderDetails(string items, int quantity, int type, bool approved,long orderID,string user)
        {
          
            try
            {
                var order = db.Orders.FirstOrDefault(c => c.OrderID == orderID);
                var UserBranch = db.UserBranches.Where(c => c.IsActive && c.UserID == user).Include(c=>c.Branch).FirstOrDefault();
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
                    WorkflowID = 1,/*order.StatusID == 2 ? 2 : 1*/
                    Maintenance=order.WithMaintenance.HasValue ? order.WithMaintenance.Value : false
                };
                db.OrderDetails.Add(orderDetails);               
                db.SaveChanges();
                orderDetails.Prefix = $"{UserBranch.Branch.BranchIP} : {order.OrderID} : {orderDetails.OrderDetailsID}";
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
