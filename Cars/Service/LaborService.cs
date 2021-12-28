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
    public class LaborService
    {
        public CarsContext db { get; set; }
        public LaborService(CarsContext carsContext)
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
        public PagingViewModel<OrderDetails> getOrderLines(int currentPage)
        {
            var orders = db.OrderDetails.Where(c => c.Enabled.HasValue && c.Price.Value > 0  && c.Enabled.Value && string.IsNullOrEmpty(c.DeletedByUserID)).Include("OrderDetailsType").Skip((currentPage - 1) * TablesMaxRows.IndexOrderLinesMaxRows).Take(TablesMaxRows.IndexOrderLinesMaxRows).ToList();

            PagingViewModel<OrderDetails> viewModel = new PagingViewModel<OrderDetails>();
            //var Brands = unitOfWork.Brands.
            //   FindAll(null, (currentPage - 1) * TablesMaxRows.InventoryBrandIndex, TablesMaxRows.InventoryBrandIndex, d => d.Name, OrderBy.Ascending);
            viewModel.items = orders.ToList();
            var itemsCount = db.OrderDetails.Where(c => c.Enabled.HasValue && c.Enabled.Value && string.IsNullOrEmpty(c.DeletedByUserID)).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexOrderLinesMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexOrderLinesMaxRows;
            return viewModel;
        }

        public PagingViewModel<Order> getOrders(int currentPage)
        {
            var orders = db.Orders.Where(c => c.Enabled.HasValue && c.Enabled.Value).
                 Include("Vehicle").Include("Customer").Include("Customer.CustomerContacts").Skip((currentPage - 1) * TablesMaxRows.IndexOrdersMaxRows).Take(TablesMaxRows.IndexOrdersMaxRows).ToList();

            PagingViewModel<Order> viewModel = new PagingViewModel<Order>();
            //var Brands = unitOfWork.Brands.
            //   FindAll(null, (currentPage - 1) * TablesMaxRows.InventoryBrandIndex, TablesMaxRows.InventoryBrandIndex, d => d.Name, OrderBy.Ascending);
            viewModel.items = orders.ToList();
            var itemsCount = db.Orders.Where(c => c.Enabled.HasValue && c.Enabled.Value).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexOrdersMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexOrdersMaxRows;
            return viewModel;
        }

        internal OrderDetails GetOrderDetailsByOrderDetailsID(long orderDetailsID)
        {
            var orderDetails = db.OrderDetails.Where(c => string.IsNullOrEmpty(c.DeletedByUserID)).Include("Order").Include("Order.Vehicle").Include("Order.Customer").Include("Order.Customer.CustomerContacts").FirstOrDefault(c => c.OrderDetailsID == orderDetailsID);
            if (string.IsNullOrEmpty(orderDetails.UsedByUser))
            {
                orderDetails.UsedByUser = "1";
                orderDetails.UsedDateTime = DateTime.Now;
                db.SaveChanges();
                return orderDetails;
            }
            if (orderDetails.UsedByUser == "1")
            {
                return orderDetails;
            }
            return null;
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
