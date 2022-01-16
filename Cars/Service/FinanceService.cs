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
    public class FinanceService
    {
        private CarsContext _context { get; set; }
        private OrderDetailsService _orderDetailsService { get; set; }
        private WorkflowOrderDetailsLogsService _workflowOrderDetailsLogsService;
        public FinanceService(CarsContext carsContext, OrderDetailsService orderDetailsService, WorkflowOrderDetailsLogsService workflowOrderDetailsLogsService)
        {
            _context = carsContext;
            _orderDetailsService = orderDetailsService;
            _workflowOrderDetailsLogsService = workflowOrderDetailsLogsService;
        }

        public async Task<PagingViewModel<OrderDetails>> GetFinanceOrderDetialsAsync(int currentPageIndex,string search)
        {
            return await _orderDetailsService.GetOrderDetailsAsync(2, 5, TablesMaxRows.IndexFinanceOrderLinesMaxRows, currentPageIndex, search);
        }

        public async Task<PagingViewModel<OrderDetails>> GetFinanceOrderDetialsWithChangelengthAsync(int currentPageIndex, int length)
        {
            TablesMaxRows.IndexFinanceOrderLinesMaxRows = length;
            return await GetFinanceOrderDetialsAsync(currentPageIndex, "");
        }

        public async Task<OrderDetails> GetFinanceOrderDetialsByIDAsync(long orderDetailsID)
        {
            var result = await _context.OrderDetails.Where(x => x.OrderDetailsID == orderDetailsID).Include(x => x.Finance).ThenInclude(x => x.FinanceDocument)
                .Include(x => x.UserBranch).ThenInclude(x => x.Branch).FirstOrDefaultAsync();
            return result;
        }

        public async Task<FinanceOrderDetailsViewModel> AssignOrderDetailsFinanceToUserAsync(long orderDetailsID, string userID)
        {
            var returnModel = new FinanceOrderDetailsViewModel();
            var orderDetailsModel = await GetFinanceOrderDetialsByIDAsync(orderDetailsID);
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
                    //Has No Finance Model so create it
                    if (orderDetailsModel.Finance == null || orderDetailsModel.FinanceID <= 0)
                    {
                        orderDetailsModel.Finance = new Finance()
                        {
                            Confirmed = true,
                            DTsCreate = DateTime.UtcNow,
                            SystemUserCreate = userID,
                        };
                    }
                    else
                    {
                        orderDetailsModel.Finance.DTsUpdate = DateTime.UtcNow;
                        orderDetailsModel.Finance.SystemUserUpdate = userID;
                    }
                    var result = await _orderDetailsService.UpdateAsync(orderDetailsModel);

                    //Create Log 
                    var log = new WorkflowOrderDetailsLog()
                    {
                        Active = true,
                        Details = "Assign Order Details to User at Finnace",
                        DTsCreate = DateTime.UtcNow,
                        OrderDetailsID = orderDetailsModel.OrderDetailsID,
                        SystemUserID = userID,
                        WorkflowID = 5
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

        public async Task<int> ReleaseOrderDetailsFinanceFromUserAsync(long orderDetailsID, string userID)
        {
            var orderDetailsModel = await GetFinanceOrderDetialsByIDAsync(orderDetailsID);
            if (orderDetailsModel == null)
                return 0;

            orderDetailsModel.UsedByUser = null;
            orderDetailsModel.UsedDateTime = null;
            var result = await _orderDetailsService.UpdateAsync(orderDetailsModel);

            //Create Log 
            var log = new WorkflowOrderDetailsLog()
            {
                Active = true,
                Details = "Release Order Details from User at Finnace",
                DTsCreate = DateTime.UtcNow,
                OrderDetailsID = orderDetailsModel.OrderDetailsID,
                SystemUserID = userID,
                WorkflowID = 5
            };
            await _workflowOrderDetailsLogsService.AddAsync(log);
            if (result == null)
                return -1;
            else
                return 1;
        }

        public async Task<int> SubmotOrderLineandMoveToNextWorkflowAsync(long orderDetailsID, string userID)
        {
            var orderDetailsModel = await GetFinanceOrderDetialsByIDAsync(orderDetailsID);
            if (orderDetailsModel == null)
                return 0;

            orderDetailsModel.UsedByUser = null;
            orderDetailsModel.UsedDateTime = null;
            orderDetailsModel.WorkflowID = 6;
            var result = await _orderDetailsService.UpdateAsync(orderDetailsModel);

            //Create Log 
            var log = new WorkflowOrderDetailsLog()
            {
                Active = true,
                Details = "Release Order Details from User at Finnace, Move to Next WorkFlow = 6",
                DTsCreate = DateTime.UtcNow,
                OrderDetailsID = orderDetailsModel.OrderDetailsID,
                SystemUserID = userID,
                WorkflowID = 5
            };
            await _workflowOrderDetailsLogsService.AddAsync(log);
            if (result == null)
                return -1;
            else
                return 1;
        }

        public async Task AddFilesAsync(IEnumerable<FinanceDocument> filePaths)
        {
            filePaths = filePaths.Select(x =>
            {
                x.DTsCreate = DateTime.UtcNow;
                return x;
            });
            await _context.FinanceDocuments.AddRangeAsync(filePaths);
            _context.SaveChanges();
        }
    }
}
