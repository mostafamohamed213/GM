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
    public class DeliveryService
    {
        public CarsContext db { get; set; }
        public DeliveryService(CarsContext carsContext)
        {
            db = carsContext;
        }


        public PagingViewModel<OrderDetails> getOrderLinesWithChangelength(int currentPageIndex, int length)
        {
            TablesMaxRows.IndexDeliverysRows = length;
            return getOrderLines(currentPageIndex, null);
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
        internal OrderDetails GetOrderDetailsByOrderDetailsID(long orderDetailsID)
        {
            var orderDetails = db.OrderDetails.Include("Order").Include("Order.Vehicle").Include("Order.Customer").Include("Order.Customer.CustomerContacts").FirstOrDefault(c => c.OrderDetailsID == orderDetailsID);
            return orderDetails;
        }
        public PagingViewModel<OrderDetails> getOrderLines(int currentPage, string? search)
        {
            var orders = db.OrderDetails.Where(c => c.WorkflowID == 9).Include("OrderDetailsType").Skip((currentPage - 1) * TablesMaxRows.IndexDeliverysRows).Take(TablesMaxRows.IndexDeliverysRows).ToList();

            PagingViewModel<OrderDetails> viewModel = new PagingViewModel<OrderDetails>();

            var itemsCount = db.OrderDetails.Where(c => c.WorkflowID == 9).Include("OrderDetailsType").Skip((currentPage - 1) * TablesMaxRows.IndexDeliverysRows).Take(TablesMaxRows.IndexDeliverysRows).Count();

            if (search != null)
            {
                orders = db.OrderDetails.Where(c => c.WorkflowID == 9 && c.Items.Contains(search)).Include("OrderDetailsType").Skip((currentPage - 1) * TablesMaxRows.IndexDeliverysRows).Take(TablesMaxRows.IndexDeliverysRows).ToList();
                itemsCount = db.OrderDetails.Where(c => c.WorkflowID == 9 && c.Items.Contains(search)).Include("OrderDetailsType").Count();
            }
            viewModel.items = orders.ToList();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexDeliverysRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexDeliverysRows;
            return viewModel;
        }

       

    }


}
