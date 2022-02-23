using Cars.Consts;
using Cars.Models;
using Cars.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service
{
    public class SystemIssuesServices
    {
        private CarsContext db { get; set; }
        public SystemIssuesServices(CarsContext carsContext)
        {
            db = carsContext;
        }

        public PagingViewModel<OrderDetailsUsedByUserViewModel> GetOrderLinesUsed(int currentPage)
        {
            var orders = db.OrderDetails.Where(c => c.StatusID != 1 && c.StatusID != 5 && (!string.IsNullOrEmpty(c.UsedByUser)|| !string.IsNullOrEmpty(c.UsedByUser2))).Include(c=>c.User).Include(c=>c.UserBranch.Branch).Include("OrderDetailsType").Skip((currentPage - 1) * TablesMaxRows.IndexOrderLinesUsedMaxRows).Take(TablesMaxRows.IndexOrderLinesUsedMaxRows)
                .Select(c=> new OrderDetailsUsedByUserViewModel {orderDetails = c,user=db.Users.FirstOrDefault(x => x.Id == c.UsedByUser || x.Id == c.UsedByUser2) }).ToList();

            PagingViewModel<OrderDetailsUsedByUserViewModel> viewModel = new PagingViewModel<OrderDetailsUsedByUserViewModel>();
            //var Brands = unitOfWork.Brands.
            //   FindAll(null, (currentPage - 1) * TablesMaxRows.InventoryBrandIndex, TablesMaxRows.InventoryBrandIndex, d => d.Name, OrderBy.Ascending);
            viewModel.items = orders.ToList();
            var itemsCount = db.OrderDetails.Where(c => c.StatusID != 1 && c.StatusID != 5 && (!string.IsNullOrEmpty(c.UsedByUser) || !string.IsNullOrEmpty(c.UsedByUser2))).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexOrderLinesUsedMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexOrderLinesUsedMaxRows;
            return viewModel;
        }

        internal PagingViewModel<OrderDetailsUsedByUserViewModel> SearchOrderLines(string search)
        {
            var orders = db.OrderDetails.Where(c => c.StatusID != 1 && c.StatusID != 5 && !string.IsNullOrEmpty(c.UsedByUser) && c.Items.Trim().ToLower().Contains(search.Trim().ToLower())).
               Include("OrderDetailsType").Include(c => c.UserBranch.Branch).Take(100).Select(c => new OrderDetailsUsedByUserViewModel { orderDetails = c, user = db.Users.FirstOrDefault(x => x.Id == c.UsedByUser || x.Id == c.UsedByUser2) }).ToList();
            PagingViewModel<OrderDetailsUsedByUserViewModel> viewModel = new PagingViewModel<OrderDetailsUsedByUserViewModel>();
            viewModel.items = orders.ToList();
            var itemsCount = orders.Count;
            double pageCount = 1;
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = 1;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = 100;
            return viewModel;
        }
        public PagingViewModel<OrderDetailsUsedByUserViewModel> getOrderLinesUsedWithChangelength(int currentPageIndex, int length)
        {
            TablesMaxRows.IndexOrderLinesUsedMaxRows = length;
            return GetOrderLinesUsed(currentPageIndex);
        }

        internal long OpenOrderDetails(long orderDetailsID)
        {
            try
            {
                var orderDetails = db.OrderDetails.FirstOrDefault(c => c.OrderDetailsID == orderDetailsID);
                orderDetails.UsedByUser = null;
                orderDetails.UsedDateTime = null;
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
        internal OrderDetailsUsedByUserViewModel GetOrderDetailsByOrderDetailsID(long orderDetailsID)
        {
            var orderDetails = db.OrderDetails.Where(c => c.StatusID != 5 && c.OrderDetailsID == orderDetailsID).Include(c => c.UserBranch.Branch).Include("OrderDetailsType").Include("Order").Include(c=>c.Order.UserBranch.Branch).Include("Order.Vehicle").Include("Order.Customer").Include("Order.Customer.CustomerContacts")
                .Select(c => new OrderDetailsUsedByUserViewModel { orderDetails = c, user = db.Users.FirstOrDefault(x => x.Id == c.UsedByUser || x.Id == c.UsedByUser2) }).FirstOrDefault();
           
          
            return orderDetails;
        }


    }
}
