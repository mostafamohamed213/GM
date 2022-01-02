﻿using Cars.Models;
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
