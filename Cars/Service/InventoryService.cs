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
    public class InventoryService
    {
        private CarsContext _context { get; set; }
        private OrderDetailsService _orderDetailsService { get; set; }
        private WorkflowOrderDetailsLogsService _workflowOrderDetailsLogsService;
        public InventoryService(CarsContext carsContext, OrderDetailsService orderDetailsService, WorkflowOrderDetailsLogsService workflowOrderDetailsLogsService)
        {
            _context = carsContext;
            _orderDetailsService = orderDetailsService;
            _workflowOrderDetailsLogsService = workflowOrderDetailsLogsService;
        }

        public async Task<PagingViewModel<OrderDetails>> GetInventoryOrderDetialsAsync(int currentPageIndex, string search)
        {
            return await _orderDetailsService.GetOrderDetailsWithStatusAndInventoryAsync(2, 7, TablesMaxRows.IndexInventoryOrderLinesMaxRows, currentPageIndex, search);
        }

        public async Task<PagingViewModel<OrderDetails>> GetInventoryOrderDetialsWithChangelengthAsync(int currentPageIndex, int length)
        {
            TablesMaxRows.IndexInventoryOrderLinesMaxRows = length;
            return await GetInventoryOrderDetialsAsync(currentPageIndex, "");
        }

        public async Task<OrderDetails> GetInventoryOrderDetialsByIDAsync(long orderDetailsID)
        {
            var result = await _context.OrderDetails.Where(x => x.OrderDetailsID == orderDetailsID).Include(x => x.Inventory).ThenInclude(x => x.InventoryDocument)
                .Include(x => x.UserBranch).ThenInclude(x => x.Branch).Include(x => x.Status).Include(x => x.Inventory).FirstOrDefaultAsync();
            return result;
        }

        public async Task<FinanceOrderDetailsViewModel> AssignOrderDetailsToUserAsync(long orderDetailsID, string userID)
        {
            var returnModel = new FinanceOrderDetailsViewModel();
            var orderDetailsModel = await GetInventoryOrderDetialsByIDAsync(orderDetailsID);
            if (orderDetailsModel == null)
                returnModel.Status = 0;
            else
            {
                if ((orderDetailsModel.UsedByUser != null || !string.IsNullOrEmpty(orderDetailsModel.UsedByUser)) && orderDetailsModel.UsedByUser != userID)
                    returnModel.Status = -1;
                else
                {
                    orderDetailsModel.UsedByUser = userID;
                    orderDetailsModel.UsedDateTime = DateTime.UtcNow;
                    //Has No Inventry Model so create it
                    if (orderDetailsModel.Inventory == null || orderDetailsModel.InventoryID <= 0)
                    {
                        orderDetailsModel.Inventory = new Inventory()
                        {
                            Confirmed = true,
                            DTsCreate = DateTime.UtcNow,
                            SystemUserCreate = userID,
                        };
                    }
                    else
                    {
                        orderDetailsModel.Inventory.DTsUpdate = DateTime.UtcNow;
                        orderDetailsModel.Inventory.SystemUserUpdate = userID;
                    }
                    var result = await _orderDetailsService.UpdateAsync(orderDetailsModel);

                    //Create Log 
                    var log = new WorkflowOrderDetailsLog()
                    {
                        Active = true,
                        Details = "Assign Order Details to User at Inventory",
                        DTsCreate = DateTime.UtcNow,
                        OrderDetailsID = orderDetailsModel.OrderDetailsID,
                        SystemUserID = userID,
                        WorkflowID = 7
                    };
                    await _workflowOrderDetailsLogsService.AddAsync(log);

                    if (result == null)
                        returnModel.Status = -2;
                    else
                    {
                        returnModel.Status = 1;
                        returnModel.model = result;
                    }
                }
            }
            return returnModel;
        }

        public async Task<int> ReleaseOrderDetailsFromUserAsync(long orderDetailsID, string userID)
        {
            var orderDetailsModel = await GetInventoryOrderDetialsByIDAsync(orderDetailsID);
            if (orderDetailsModel == null)
                return 0;

            orderDetailsModel.UsedByUser = null;
            orderDetailsModel.UsedDateTime = null;
            var result = await _orderDetailsService.UpdateAsync(orderDetailsModel);

            //Create Log 
            var log = new WorkflowOrderDetailsLog()
            {
                Active = true,
                Details = "Release Order Details from User at Inventory",
                DTsCreate = DateTime.UtcNow,
                OrderDetailsID = orderDetailsModel.OrderDetailsID,
                SystemUserID = userID,
                WorkflowID = 7
            };
            await _workflowOrderDetailsLogsService.AddAsync(log);
            if (result == null)
                return -1;
            else
                return 1;
        }

        public async Task<int> ReleaseOrderDetailsFromUserAndUpdateQuntityAsync(long orderDetailsID,int currentQuantity, string userID)
        {
            var orderDetailsModel = await GetInventoryOrderDetialsByIDAsync(orderDetailsID);
            if (orderDetailsModel == null)
                return 0;
            int oldQuantity = orderDetailsModel.Inventory.Quantity;
            orderDetailsModel.UsedByUser = null;
            orderDetailsModel.UsedDateTime = null;
            orderDetailsModel.Inventory.Quantity = currentQuantity;
            var result = await _orderDetailsService.UpdateAsync(orderDetailsModel);

            //Create Log 
            var log = new WorkflowOrderDetailsLog()
            {
                Active = true,
                Details = $"Release Order Details from User at Inventory, Update Quantity From {oldQuantity} To {currentQuantity}",
                DTsCreate = DateTime.UtcNow,
                OrderDetailsID = orderDetailsModel.OrderDetailsID,
                SystemUserID = userID,
                WorkflowID = 7
            };
            await _workflowOrderDetailsLogsService.AddAsync(log);
            if (result == null)
                return -1;
            else
                return 1;
        }

        public async Task<int> ReleaseDoneOrderDetailsFromUserAsync(long orderDetailsID, string userID)
        {
            var orderDetailsModel = await GetInventoryOrderDetialsByIDAsync(orderDetailsID);
            if (orderDetailsModel == null)
                return 0;

            orderDetailsModel.UsedByUser = null;
            orderDetailsModel.UsedDateTime = null;
            orderDetailsModel.StatusID = 3;
            orderDetailsModel.WorkflowID = 8;
            var result = await _orderDetailsService.UpdateAsync(orderDetailsModel);

            //Create Log 
            var log = new WorkflowOrderDetailsLog()
            {
                Active = true,
                Details = $"Release Order Details from User at Inventory, Make Order Details Done",
                DTsCreate = DateTime.UtcNow,
                OrderDetailsID = orderDetailsModel.OrderDetailsID,
                SystemUserID = userID,
                WorkflowID = 7
            };
            await _workflowOrderDetailsLogsService.AddAsync(log);
            if (result == null)
                return -1;
            else
                return 1;
        }

        public async Task<int> ReleaseOrderDetailsFromUserAndRejectAsync(long orderDetailsID, string userID)
        {
            var orderDetailsModel = await GetInventoryOrderDetialsByIDAsync(orderDetailsID);
            if (orderDetailsModel == null)
                return 0; 
            
            OrderDetails newOrder = new OrderDetails()
            {
                Comments = orderDetailsModel.Comments,
                OrderDetailsTypeID = orderDetailsModel.OrderDetailsTypeID,
                Labor_Value = orderDetailsModel.Labor_Value,
                DTsCreate = DateTime.UtcNow,
                FinanceID = orderDetailsModel.FinanceID,
                InventoryID = orderDetailsModel.InventoryID,
                IsApproved = orderDetailsModel.IsApproved,
                Items = orderDetailsModel.Items,
                Labor_Hours = orderDetailsModel.Labor_Hours,
                OrderID = orderDetailsModel.OrderID,
                ParentOrderDetailsID = orderDetailsModel.ParentOrderDetailsID,
                PartNumber = orderDetailsModel.PartNumber,
                Prefix = orderDetailsModel.Prefix,
                Quantity = orderDetailsModel.Quantity,
                Price = orderDetailsModel.Price,
                QuotationID = orderDetailsModel.QuotationID,
                RunnerID = orderDetailsModel.RunnerID,
                StatusID = orderDetailsModel.StatusID,
                SystemUserCreate = userID,
                UserBranchID = orderDetailsModel.UserBranchID,
                VendorLocationID = orderDetailsModel.VendorLocationID,
                WorkflowID = orderDetailsModel.WorkflowID
            };

            var addNewOrderResult = await _orderDetailsService.AddAsync(newOrder);
            if (addNewOrderResult == null)
                return 0;

            orderDetailsModel.UsedByUser = null;
            orderDetailsModel.UsedDateTime = null;
            orderDetailsModel.StatusID = 6;
            orderDetailsModel.WorkflowID = 8;
            var result = await _orderDetailsService.UpdateAsync(orderDetailsModel);

            //Create Log 
            var log = new WorkflowOrderDetailsLog()
            {
                Active = true,
                Details = $"Release Order Details from User at Inventory, And Reject Order With ID = {orderDetailsModel.OrderDetailsID}, Create new one with same details with ID = {addNewOrderResult.OrderDetailsID}",
                DTsCreate = DateTime.UtcNow,
                OrderDetailsID = orderDetailsModel.OrderDetailsID,
                SystemUserID = userID,
                WorkflowID = 7
            };
            await _workflowOrderDetailsLogsService.AddAsync(log);
            if (result == null)
                return -1;
            else
                return 1;
        }

        public async Task AddFilesAsync(IEnumerable<InventoryDocument> filePaths)
        {
            filePaths = filePaths.Select(x =>
            {
                x.DTsCreate = DateTime.UtcNow;
                return x;
            });
            await _context.InventoryDocuments.AddRangeAsync(filePaths);
            _context.SaveChanges();
        }
    }
}
