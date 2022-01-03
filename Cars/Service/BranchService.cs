using Cars.Controllers;
using Cars.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service
{
    public class BranchService
    {
        public CarsContext _context { get; set; }
        public BranchService(CarsContext carsContext)
        {
            _context = carsContext;
        }

        public async Task<PaginatedList<BranchModel>> GetAllAsync(string currentFilter, string searchString, int? pageNumber, int pageSize)
        {
            try
            {
                if (searchString != null)
                    pageNumber = 1;
                else
                    searchString = currentFilter;

                IQueryable<BranchModel> branches;

                //Search by name, IP
                if (!String.IsNullOrEmpty(searchString))
                    branches = _context.Branches.Where(s => s.Name.ToLower().Trim().Contains(searchString.ToLower().Trim()) || s.BranchIP.ToLower().Trim().Contains(searchString.ToLower().Trim())).AsNoTracking();
                else
                    branches = _context.Branches.AsNoTracking();

                return await PaginatedList<BranchModel>.CreateAsync(branches, pageNumber ?? 1, pageSize);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<IEnumerable<BranchModel>> GetAllActiveNotUsedAsync(string userID, int? userBranchID)
        {
            try
            {
                //Exclude Curent Branch from List 
                if (userBranchID != null && userBranchID > 0)
                    return await _context.Branches.Where(x => x.IsActive == true && !_context.UserBranches.Where(y => y.UserID == userID && y.UserBranchID != userBranchID).Select(y => y.BranchID).Contains(x.BranchID))
                    .AsNoTracking().ToListAsync();
                return await _context.Branches.Where(x => x.IsActive == true && !_context.UserBranches.Where(y => y.UserID == userID).Select(y => y.BranchID).Contains(x.BranchID))
                .AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<BranchModel> GetByIDAsync(int branchID)
        {
            try
            {
                return await _context.Branches.AsNoTracking()
                .FirstOrDefaultAsync(m => m.BranchID == branchID);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<BranchModel> GetAsync(string name, string branchIP)
        {
            try
            {
                return await _context.Branches.AsNoTracking().FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower() || x.BranchIP == branchIP);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<BranchModel> GetAsync(string name, string branchIP, int excludedBranchID)
        {
            try
            {
                return await _context.Branches.AsNoTracking().FirstOrDefaultAsync(x => (x.Name.ToLower() == name.ToLower() || x.BranchIP == branchIP) && x.BranchID != excludedBranchID);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> IsBranchExistsAsync(int id)
        {
            try
            {
                return await _context.Branches.AnyAsync(e => e.BranchID == id);

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<BranchModel> AddAsync(BranchModel model)
        {
            try
            {
                model.DTsCreate = DateTime.UtcNow;
                model.IsActive = true;
                var result = await _context.AddAsync(model);
                await _context.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<BranchModel> UpdateAsync(BranchModel model)
        {
            try
            {
                model.DTsUpdate = DateTime.UtcNow;
                var result = _context.Update(model);
                await _context.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
