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
                orderDetails = await _context.OrderDetails.Where(c => c.StatusID == statusID && c.WorkflowID == workflowID).Include(x => x.UserBranch).ThenInclude(x=>x.Branch).Skip((currentPage - 1) * maxRows).Take(maxRows).ToListAsync();
                itemsCount = _context.OrderDetails.Where(c => c.StatusID == statusID && c.WorkflowID == workflowID).Count();
            }
            else
            {
                orderDetails = await _context.OrderDetails.Where(c => c.StatusID == statusID && c.WorkflowID == workflowID
                        && (c.Items.ToLower().Trim().Contains(search.ToLower().Trim()) || c.Quantity.ToString().Contains(search.ToLower().Trim()) || (c.Prefix).Contains(search.ToLower().Trim())))
                        .Skip((currentPage - 1) * maxRows).Take(maxRows).Include(x => x.UserBranch).ThenInclude(x => x.Branch).ToListAsync();
                itemsCount = _context.OrderDetails.Where(c => c.StatusID == statusID && c.WorkflowID == workflowID
                        && (c.Items.ToLower().Trim().Contains(search.ToLower().Trim()) || c.Quantity.ToString().Contains(search.ToLower().Trim()) || (c.Prefix).Contains(search.ToLower().Trim()))).Count();
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

        /// <summary>
        /// Get All Orders Which Added to workflow after spesific duration 
        /// </summary>
        /// <param name="statusID"></param>
        /// <param name="workflowID"></param>
        /// <param name="duration">In Hours</param>
        /// <returns></returns>
        public async Task<IEnumerable<OrderDetails>> GetOrderDetailsAsync(int statusID, int workflowID, string duration)
        {
            var orderDetails = await _context.OrderDetails.Where(c => c.StatusID == statusID && c.WorkflowID == workflowID
                                                                    && c.DTsWorflowEnter).ToListAsync();
            return orderDetails;
        }

        public async Task<PagingViewModel<OrderDetails>> GetRunnerOrderDetailsAsync(string userID, int maxRows, int currentPage, string search)
        {
            List<OrderDetails> orderDetails = null;
            int itemsCount = 0;
            if (string.IsNullOrEmpty(search))
            {
                orderDetails = await _context.OrderDetails.Where(c => c.RunnerID == userID && c.RunnerID == userID && c.StatusID == 2 && (c.WorkflowID == 6 || c.WorkflowID == 7)).Include(x => x.UserBranch).ThenInclude(x => x.Branch).Include(x => x.Workflow)
                    .Skip((currentPage - 1) * maxRows).Take(maxRows).ToListAsync();
                itemsCount = _context.OrderDetails.Where(c => c.RunnerID == userID && c.RunnerID == userID && c.StatusID == 2 && (c.WorkflowID == 6 || c.WorkflowID == 7)).Count();
            }
            else
            {
                orderDetails = await _context.OrderDetails.Where(c => c.RunnerID == userID && c.RunnerID == userID && c.StatusID == 2 && (c.WorkflowID == 6 || c.WorkflowID == 7)
                        && (c.Items.ToLower().Trim().Contains(search.ToLower().Trim()) || c.Quantity.ToString().Contains(search.ToLower().Trim()) || (c.Prefix).Contains(search.ToLower().Trim())))
                        .Skip((currentPage - 1) * maxRows).Take(maxRows).Include(x => x.UserBranch).ThenInclude(x => x.Branch).Include(x => x.Workflow).ToListAsync();
                itemsCount = _context.OrderDetails.Where(c => c.RunnerID == userID && c.RunnerID == userID && c.StatusID == 2 && (c.WorkflowID == 6 || c.WorkflowID == 7)
                        && (c.Items.ToLower().Trim().Contains(search.ToLower().Trim()) || c.Quantity.ToString().Contains(search.ToLower().Trim()) || (c.Prefix).Contains(search.ToLower().Trim()))).Count();
            }
            PagingViewModel<OrderDetails> viewModel = new PagingViewModel<OrderDetails>();
            viewModel.items = orderDetails.OrderByDescending(x => x.DTsCreate).ToList();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(maxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = maxRows;
            return viewModel;
        }

        public async Task<OrderDetails> GetByIDAsync(long orderDetailsID, string userID)
        {
            var result = await _context.OrderDetails.Where(x => x.RunnerID==userID && x.OrderDetailsID == orderDetailsID).Include(x => x.UserBranch).ThenInclude(x => x.Branch).Include(x => x.Workflow).FirstOrDefaultAsync();
            return result;
        }

        public async Task<PagingViewModel<OrderDetails>> GetOrderDetailsWithStatusAndInventoryAsync(int statusID, int workflowID, int maxRows, int currentPage, string search)
        {
            List<OrderDetails> orderDetails = null;
            int itemsCount = 0;
            if (string.IsNullOrEmpty(search))
            {
                orderDetails = await _context.OrderDetails.Where(c => c.StatusID == statusID && c.WorkflowID == workflowID).Include(x => x.UserBranch).ThenInclude(x => x.Branch).Include(x => x.Status).Include(x => x.Inventory)
                    .Skip((currentPage - 1) * maxRows).Take(maxRows).ToListAsync();
                itemsCount = _context.OrderDetails.Where(c => c.StatusID == statusID && c.WorkflowID == workflowID).Count();
            }
            else
            {
                orderDetails = await _context.OrderDetails.Where(c => c.StatusID == statusID && c.WorkflowID == workflowID
                        && (c.Items.ToLower().Trim().Contains(search.ToLower().Trim()) || c.Quantity.ToString().Contains(search.ToLower().Trim()) || (c.Prefix).Contains(search.ToLower().Trim())))
                        .Skip((currentPage - 1) * maxRows).Take(maxRows).Include(x => x.UserBranch).ThenInclude(x => x.Branch).Include(x => x.Status).Include(x => x.Inventory).ToListAsync();
                itemsCount = _context.OrderDetails.Where(c => c.StatusID == statusID && c.WorkflowID == workflowID
                        && (c.Items.ToLower().Trim().Contains(search.ToLower().Trim()) || c.Quantity.ToString().Contains(search.ToLower().Trim()) || (c.Prefix).Contains(search.ToLower().Trim()))).Count();
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
        public async Task<OrderDetails> AddAsync(OrderDetails model)
        {
            var result = await _context.OrderDetails.AddAsync(model);
            if (await _context.SaveChangesAsync() > 0)
                return result.Entity;
            else
                return null;
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
