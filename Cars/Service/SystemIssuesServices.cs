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

        public PagingViewModel<OrderDetails> GetOrderLinesUsed(int currentPage)
        {
            var orders = db.OrderDetails.Where(c => c.StatusID != 1 && c.StatusID != 5 && !string.IsNullOrEmpty(c.UsedByUser)).Include("OrderDetailsType").Skip((currentPage - 1) * TablesMaxRows.IndexOrderLinesUsedMaxRows).Take(TablesMaxRows.IndexOrderLinesUsedMaxRows).ToList();

            PagingViewModel<OrderDetails> viewModel = new PagingViewModel<OrderDetails>();
            //var Brands = unitOfWork.Brands.
            //   FindAll(null, (currentPage - 1) * TablesMaxRows.InventoryBrandIndex, TablesMaxRows.InventoryBrandIndex, d => d.Name, OrderBy.Ascending);
            viewModel.items = orders.ToList();
            var itemsCount = db.OrderDetails.Where(c => c.StatusID != 1 && c.StatusID != 5 && !string.IsNullOrEmpty(c.UsedByUser)).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexOrderLinesUsedMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexOrderLinesUsedMaxRows;
            return viewModel;
        }
        public PagingViewModel<OrderDetails> getOrderLinesUsedWithChangelength(int currentPageIndex, int length)
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
                db.SaveChanges();
                return orderDetails.OrderDetailsID;
            }
            catch (Exception)
            {

                return 0;
            }
        }
        internal OrderDetails GetOrderDetailsByOrderDetailsID(long orderDetailsID)
        {
            var orderDetails = db.OrderDetails.Where(c => c.StatusID != 5).Include("OrderDetailsType").Include("Order").Include("Order.Vehicle").Include("Order.Customer").Include("Order.Customer.CustomerContacts").FirstOrDefault(c => c.OrderDetailsID == orderDetailsID);
           
          
            return orderDetails;
        }
    }
}
