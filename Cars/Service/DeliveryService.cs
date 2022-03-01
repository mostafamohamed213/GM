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


        public PagingViewModel<OrderDetails> getOrderLinesWithChangelength(int currentPageIndex, int length,string DeliverID)
        {
            TablesMaxRows.IndexDeliverysRows = length;
            return getOrderLines(currentPageIndex, null,DeliverID);
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
            var orderDetails = db.OrderDetails.Where(c => c.OrderDetailsID == orderDetailsID).Include(c=>c.Order.UserBranch.Branch).Include("Order").Include("Order.Vehicle").Include("Order.Customer").Include("Order.Customer.CustomerContacts").Include(c=>c.VendorLocation).FirstOrDefault();
            return orderDetails;
        }
        public PagingViewModel<OrderDetails> getOrderLines(int currentPage, string search ,string DeliveryID)
        {
            var orders = db.OrderDetails.Where(c => c.WorkflowID == 1 && c.DeliveryID== DeliveryID).Include("Order.Vehicle").Include("OrderDetailsType").Skip((currentPage - 1) * TablesMaxRows.IndexDeliverysRows).Take(TablesMaxRows.IndexDeliverysRows).ToList();

            PagingViewModel<OrderDetails> viewModel = new PagingViewModel<OrderDetails>();

            var itemsCount = db.OrderDetails.Where(c => c.WorkflowID == 1 && c.DeliveryID == DeliveryID).Skip((currentPage - 1) * TablesMaxRows.IndexDeliverysRows).Take(TablesMaxRows.IndexDeliverysRows).Count();

            if (search != null)
            {
                orders = db.OrderDetails.Where(c => c.WorkflowID == 1 && c.DeliveryID == DeliveryID && c.Items.Contains(search)).Include("Order.Vehicle").Include("OrderDetailsType").Skip((currentPage - 1) * TablesMaxRows.IndexDeliverysRows).Take(TablesMaxRows.IndexDeliverysRows).ToList();
                itemsCount = db.OrderDetails.Where(c => c.WorkflowID == 1 && c.DeliveryID == DeliveryID && c.Items.Contains(search)).Count();
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
