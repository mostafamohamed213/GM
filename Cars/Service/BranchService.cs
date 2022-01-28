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

        public async Task<IEnumerable<BranchModel>> GetAllActiveAsync()
        {
            try
            {
                return await _context.Branches.Where(x => x.IsActive == true).AsNoTracking().ToListAsync();
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

        public async Task<BranchModel> AddAsync(BranchModel model,string[] Employee)
        {
            try
            {
                model.DTsCreate = DateTime.UtcNow;
                model.IsActive = true;
                var result = await _context.AddAsync(model);
                await _context.SaveChangesAsync();
                List<UserBranchModel> employees = new List<UserBranchModel>();


                foreach (var item in Employee)
                {
                  var x=  _context.UserBranches.Where(a => a.UserID == item && a.IsActive==true).ToList();
                    if (x.Count != 0)
                    {
                        x.ForEach(a => { a.IsActive = false; a.DTsEnd = DateTime.Now; });
                        await _context.SaveChangesAsync();
                    }
                
                

                    employees.Add(new UserBranchModel() { BranchID=result.Entity.BranchID, UserID =item ,IsActive=true,DTsCreate=DateTime.Now });
                 
                }
                _context.UserBranches.AddRange(employees);
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
                List<UserBranchModel> employees = new List<UserBranchModel>();
                List<UserBranchModel> x = _context.UserBranches.Where(e => e.BranchID == model.BranchID && e.IsActive == true).ToList();
                if (x.Count !=0)
                {
                    x.ForEach(a => { a.IsActive = false; a.DTsEnd = DateTime.Now; });
                    await _context.SaveChangesAsync();
                }
                if (model.employee != null)
                {
                    foreach (var item in model.employee)
                    {
                        var y = _context.UserBranches.Where(a => a.UserID == item && a.IsActive==true).ToList();
                        if (y.Count != 0)
                        {
                            y.ForEach(a => { a.IsActive = false; a.DTsEnd = DateTime.Now; });
                            await _context.SaveChangesAsync();
                        }

                        employees.Add(new UserBranchModel() { BranchID = result.Entity.BranchID, UserID = item, IsActive = true, DTsCreate = DateTime.Now });
            
                    }
                    _context.UserBranches.AddRange(employees);
                    await _context.SaveChangesAsync();
                }

                return result.Entity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
