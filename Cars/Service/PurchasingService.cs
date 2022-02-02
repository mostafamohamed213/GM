using Cars.Consts;
using Cars.Models;
using Cars.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service
{
    public class PurchasingService
    {
        public CarsContext db { get; set; }

        public PurchasingService(CarsContext context)
        {
            db = context;
        }
        public PagingViewModel<OrderDetails> getOrderDetails(int currentPage)
        {
            var orderDetails = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 6).Include(c => c.UserBranch.Branch).Include(c=>c.OrderDetailsType).Include(c=>c.VendorLocation).Skip((currentPage - 1) * TablesMaxRows.IndexPurchasingMaxRows).Take(TablesMaxRows.IndexPurchasingMaxRows).ToList();
            PagingViewModel<OrderDetails> viewModel = new PagingViewModel<OrderDetails>();
            viewModel.items = orderDetails.ToList();
            var itemsCount = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 6).Where(c => c.StatusID == 2).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexPurchasingMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexPurchasingMaxRows;
            return viewModel;
        }
        internal PagingViewModel<OrderDetails> SearchOrderLines(string search)
        {
            var orders = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 6 && c.Items.Trim().ToLower().Contains(search.Trim().ToLower())).
              Include(c => c.OrderDetailsType).Include(c => c.VendorLocation).Include(c => c.UserBranch.Branch).Take(100).ToList();
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

        internal SelectList getRunners()
        {
          
          var usersId = db.UserRoles.Where(c => c.RoleId == db.Roles.FirstOrDefault(a => a.Name == "Runner" || a.Name == "Runners").Id).Select(c=>c.UserId).ToList();
          var Runners = db.Users.Where(c => usersId.Contains(c.Id)).Select(c=> new { ID = c.Id,Name =c.FirstName+" "+c.SeconedName }).ToList();
            if (Runners.Count() > 0)
            {
                return new SelectList(Runners, "ID", "Name");             
            }
            return null;
        }

        internal OrderDetails getOrderDetailsByID(long orderDetailsID)
        {
          return db.OrderDetails.Include(c => c.UserBranch.Branch).Include(c => c.OrderDetailsType).Include(c => c.VendorLocation).FirstOrDefault(c => c.OrderDetailsID == orderDetailsID);
        }

        public PagingViewModel<OrderDetails> getOrdersWithChangelength(int currentPageIndex, int length)
        {
            TablesMaxRows.IndexPurchasingMaxRows = length;
            return getOrderDetails(currentPageIndex);
        }

        internal int AssignVendor(long orderDetailsID, string runnerID,string user)
        {
            OrderDetails orderDetails = db.OrderDetails.FirstOrDefault(c=>c.OrderDetailsID == orderDetailsID);
            if (orderDetails is not null)
            {
                orderDetails.RunnerID = runnerID;
                orderDetails.WorkflowID = 7;
                db.WorkflowOrderDetailsLogs.Add(new WorkflowOrderDetailsLog
                {
                    DTsCreate = DateTime.Now,
                    Active = true,
                    OrderDetailsID = orderDetails.OrderDetailsID,
                    SystemUserID = user,
                    WorkflowID = 6,
                });
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        readonly object _object = new object();
        internal OrderDetails CloseOrderDetails(long orderDetailsID,string user)
        {
            try
            {
                lock (_object)
                {
                    var orderDetails = getOrderDetailsByID(orderDetailsID);
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

        internal int CancelOrderDetails(CancelOrderDetailsViewModel model,string user)
        {
            
            OrderDetails orderDetails = db.OrderDetails.FirstOrDefault(c=>c.OrderDetailsID == model.OrderDetailsID);
            orderDetails.StatusID = 4;         
            OrderDetailsStatusLog log = new OrderDetailsStatusLog
            {
                DTsCreate = DateTime.Now,
                SystemUserID = user,
                OrderDetailsID=orderDetails.OrderDetailsID,
                WorkflowID =orderDetails.WorkflowID,
                Reason=model.Reason,
                StatusID = 4,
                Detatils=model.Detatils                
            };
            db.OrderDetailsStatusLogs.Add(log);
            db.SaveChanges();
            if (model.FormFiles.Length > 0)
            {
                string path = $"wwwroot/Resources/Status/Cancel/{orderDetails.OrderDetailsID}";
                if (!(Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);
                }
                foreach (IFormFile file in model.FormFiles)
                {
                    var path1 = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/Resources/Status/Cancel/{orderDetails.OrderDetailsID}", file.FileName);
                    var stream = new FileStream(path1, FileMode.Create);
                    file.CopyTo(stream);
                    db.StatusLogDocuments.Add(new StatusLogDocument() { DTsCreate = DateTime.Now, Path = $"/Resources/Status/Cancel/{orderDetails.OrderDetailsID}/{file.FileName}", SystemUserID = user, Details = file.FileName,OrderDetailsStatusLogID = log.OrderDetailsStatusLogID });
                }
            }
            db.SaveChanges();
            OpenOrderDetails(model.OrderDetailsID);
            return 1;
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
