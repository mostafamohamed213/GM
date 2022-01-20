using Cars.Consts;
using Cars.Models;
using Cars.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service
{
    public class RunnerOrdersService
    {
        private OrderDetailsService _orderDetailsService { get; set; }
        public RunnerOrdersService(OrderDetailsService orderDetailsService)
        {
            _orderDetailsService = orderDetailsService;
        }

        public async Task<PagingViewModel<OrderDetails>> GetRunnerOrderDetialsAsync(string userID, int currentPageIndex, string search)
        {
            return await _orderDetailsService.GetRunnerOrderDetailsAsync(userID, TablesMaxRows.IndexRunnerOrderLinesRows, currentPageIndex, search);
        }

        public async Task<PagingViewModel<OrderDetails>> GetRunnerOrderDetialsWithChangelengthAsync(string userID, int currentPageIndex, int length)
        {
            TablesMaxRows.IndexRunnerOrderLinesRows = length;
            return await GetRunnerOrderDetialsAsync(userID, currentPageIndex, "");
        }

        public async Task<OrderDetails> GetRunnerOrderDetialsByIDAsync(long orderDetailsID)
        {
            var result = await _orderDetailsService.GetByIDAsync(orderDetailsID);
            return result;
        }
    }
}
