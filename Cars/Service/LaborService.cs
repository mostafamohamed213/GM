﻿using Cars.Consts;
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
           
            var orders = db.OrderDetails.Include(c=>c.Order).Where(c => c.StatusID == 2 && c.WorkflowID == 1 && c.Order.WithMaintenance.HasValue && c.Order.WithMaintenance.Value ).Include(c => c.VendorLocation).Include("OrderDetailsType").Skip((currentPage - 1) * TablesMaxRows.IndexLaborMaxRows).Take(TablesMaxRows.IndexLaborMaxRows).OrderByDescending(c=>c.DTsCreate).ToList();

            PagingViewModel<OrderDetails> viewModel = new PagingViewModel<OrderDetails>();
            viewModel.items = orders.ToList();
            var itemsCount = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 1 && c.Order.WithMaintenance.HasValue && c.Order.WithMaintenance.Value).Include(c => c.VendorLocation).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexLaborMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexLaborMaxRows;
            return viewModel;
        }

        public PagingViewModel<OrderDetails> getByType(int currentPage,string? type, decimal? from ,decimal? to,int? vendor)
        {
            var orders = db.OrderDetails.Include(c => c.Order).Where(c => c.StatusID == 2 && c.WorkflowID == 1 && c.Order.WithMaintenance.HasValue && c.Order.WithMaintenance.Value).Include("OrderDetailsType").Skip((currentPage - 1) * TablesMaxRows.IndexLaborMaxRows).Take(TablesMaxRows.IndexLaborMaxRows).OrderByDescending(c=>c.DTsCreate).ToList();

            
            if (type != "all" && type!=null)
            {
                orders = db.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 1 && c.Order.WithMaintenance.HasValue && c.Order.WithMaintenance.Value && c.OrderDetailsType.NameEn == type ).Include("OrderDetailsType").Skip((currentPage - 1) * TablesMaxRows.IndexLaborMaxRows).Take(TablesMaxRows.IndexLaborMaxRows).OrderByDescending(c => c.DTsCreate).ToList();
            }

            if(from!=null || to!=null || vendor != null)
            {
                if(from !=null)
                {
                    orders = orders.Where(c=> c.Price > from ).ToList();
                }
                if(to!=null)
                {
                    orders = orders.Where(c => c.Price <to).ToList();
                }
                if(vendor != null)
                {
                    orders = orders.Where(c => c.VendorLocationID==vendor).ToList();
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
        internal OrderDetails GetOrderDetailsByOrderDetailsID(long orderDetailsID)
        {
            var orderDetails = db.OrderDetails.Where(c => c.StatusID ==2 && c.WorkflowID == 1).Include("Order").Include("Order.Vehicle").Include("Order.Customer").Include("Order.Customer.CustomerContacts").FirstOrDefault(c => c.OrderDetailsID == orderDetailsID);
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

        internal long EditOrderDetailsFromSales(string items, int quantity, int type, bool approved, decimal? labor_hours, double? labor_value, long orderDetailsID,string UserId)
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
    }
}
