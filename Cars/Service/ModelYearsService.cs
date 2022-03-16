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
    public class ModelYearsService
    {
        private CarsContext _context { get; set; }
        public ModelYearsService(CarsContext carsContext)
        {
            _context = carsContext;
        }

        public async Task<PagingViewModel<ModelYear>> GetAllAsync(long modelID, int currentPageIndex, string search)
        {
            List<ModelYear> modelYears = null;
            int itemsCount = 0;
            if (string.IsNullOrEmpty(search))
            {
                modelYears = await _context.ModelYears.Where(x => x.ModelID == modelID).Include(x => x.Model).Skip((currentPageIndex - 1) * TablesMaxRows.IndexModelYearsRows).Take(TablesMaxRows.IndexModelYearsRows).ToListAsync();
                itemsCount = _context.ModelYears.Where(x => x.ModelID == modelID).Count();
            }
            else
            {
                modelYears = await _context.ModelYears.Where(c => c.ModelID == modelID && (c.Year.Trim().Contains(search.ToLower().Trim()) || c.Model.Name.ToLower().Trim().Contains(search.ToLower().Trim())))
                    .Include(x => x.Model).Skip((currentPageIndex - 1) * TablesMaxRows.IndexModelYearsRows).Take(TablesMaxRows.IndexModelYearsRows).ToListAsync();
                itemsCount = _context.ModelYears.Where(c => c.ModelID == modelID && (c.Year.Trim().Contains(search.ToLower().Trim())) || c.Model.Name.ToLower().Trim().Contains(search.ToLower().Trim())).Count();
            }
            PagingViewModel<ModelYear> viewModel = new PagingViewModel<ModelYear>();
            viewModel.items = modelYears.ToList();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexModelYearsRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPageIndex;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexModelYearsRows;
            return viewModel;
        }

        public async Task<PagingViewModel<ModelYear>> GetAllWithChangelengthAsync(long modelID, int currentPageIndex, int length)
        {
            TablesMaxRows.IndexModelYearsRows = length;
            return await GetAllAsync(modelID, currentPageIndex, "");
        }

        public async Task<ModelYear> GetByIDAsync(long id)
        {
            return await _context.ModelYears.Include(x => x.Model).ThenInclude(x => x.Brand).Where(x => x.ModelYearID == id).FirstOrDefaultAsync();
        }

        public async Task<ModelYear> AddAsync(ModelYear model)
        {
            model.DTsCreate = DateTime.Now;
            var result = await _context.ModelYears.AddAsync(model);
            return (await _context.SaveChangesAsync() > 0) ? result.Entity : null;
        }

        public async Task<int> UpdateAsync(ModelYear model)
        {
            model.DTsCreate = DateTime.Now;
            _context.ModelYears.Update(model);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(long id)
        {
            var model = await _context.ModelYears.Where(x => x.ModelYearID == id)
                .FirstOrDefaultAsync();
            _context.ModelYears.Remove(model);
            return await _context.SaveChangesAsync();
        }
    }
}
