using Cars.Models;
using Cars.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service
{
    public class OrderDetailsService
    {
        public CarsContext _context { get; set; }
        public OrderDetailsService(CarsContext carsContext)
        {
            _context = carsContext;
        }
        public async Task<PagingViewModel<OrderDetails>> GetOrderDetailsAsync(int statusID, int workflowID, int maxRows, int currentPage, string search)
        {
            List<OrderDetails> orderDetails = null;
            int itemsCount = 0;
            if (string.IsNullOrEmpty(search))
            {
                orderDetails = await _context.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 5).Include(x => x.UserBranch).Skip((currentPage - 1) * maxRows).Take(maxRows).ToListAsync();
                itemsCount = _context.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 5).Count();
            }
            else
            {
                orderDetails = await _context.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 5
                        && (c.Items.ToLower().Trim().Contains(search.ToLower().Trim()) || c.Quantity.ToString().Contains(search.ToLower().Trim()) || ("01:" + c.OrderID + ':' + c.OrderDetailsID).Contains(search.ToLower().Trim())))
                        .Skip((currentPage - 1) * maxRows).Take(maxRows).Include(x => x.UserBranch).ToListAsync();
                itemsCount = _context.OrderDetails.Where(c => c.StatusID == 2 && c.WorkflowID == 5
                        && (c.Items.ToLower().Trim().Contains(search.ToLower().Trim()) || c.Quantity.ToString().Contains(search.ToLower().Trim()) || ("01:" + c.OrderID + ':' + c.OrderDetailsID).Contains(search.ToLower().Trim()))).Count();
            }
            PagingViewModel<OrderDetails> viewModel = new PagingViewModel<OrderDetails>();
            viewModel.items = orderDetails.ToList();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(maxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = maxRows;
            return viewModel;
        }

        public async Task<OrderDetails> UpdateAsync(OrderDetails model)
        {
            var result = _context.OrderDetails.Update(model);
            if (await _context.SaveChangesAsync() > 0)
                return result.Entity;
            else
                return null;
        }
    }
}
