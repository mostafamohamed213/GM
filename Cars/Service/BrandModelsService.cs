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
    public class BrandModelsService
    {
        private CarsContext _context { get; set; }
        public BrandModelsService(CarsContext carsContext)
        {
            _context = carsContext;
        }

        public async Task<PagingViewModel<BrandModel>> GetAllAsync(long brandID, int currentPageIndex, string search)
        {
            List<BrandModel> brands = null;
            int itemsCount = 0;
            if (string.IsNullOrEmpty(search))
            {
                brands = await _context.BrandModels.Where(x => x.BrandID == brandID).Include(x => x.Brand).Skip((currentPageIndex - 1) * TablesMaxRows.IndexBrandModelsRows).Take(TablesMaxRows.IndexBrandModelsRows).ToListAsync();
                itemsCount = _context.BrandModels.Where(x => x.BrandID == brandID).Count();
            }
            else
            {
                brands = await _context.BrandModels.Where(c => c.BrandID == brandID && (c.Name.ToLower().Trim().Contains(search.ToLower().Trim()) || c.Brand.Name.ToLower().Trim().Contains(search.ToLower().Trim())))
                    .Include(x => x.Brand).Skip((currentPageIndex - 1) * TablesMaxRows.IndexBrandModelsRows).Take(TablesMaxRows.IndexBrandModelsRows).ToListAsync();
                itemsCount = _context.BrandModels.Where(c => c.BrandID == brandID && (c.Name.ToLower().Trim().Contains(search.ToLower().Trim())) || c.Brand.Name.ToLower().Trim().Contains(search.ToLower().Trim())).Count();
            }
            PagingViewModel<BrandModel> viewModel = new PagingViewModel<BrandModel>();
            viewModel.items = brands.ToList();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexBrandModelsRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPageIndex;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexBrandModelsRows;
            return viewModel;
        }

        public async Task<PagingViewModel<BrandModel>> GetAllWithChangelengthAsync(long brandID, int currentPageIndex, int length)
        {
            TablesMaxRows.IndexBrandModelsRows = length;
            return await GetAllAsync(brandID, currentPageIndex, "");
        }

        public async Task<BrandModel> GetByIDAsync(long id)
        {
            return await _context.BrandModels.Include(x => x.Brand).Where(x => x.ModelID == id).FirstOrDefaultAsync();
        }

        public async Task<BrandModel> AddAsync(BrandModel model)
        {
            model.DTsCreate = DateTime.Now;
            var result = await _context.BrandModels.AddAsync(model);
            return (await _context.SaveChangesAsync() > 0) ? result.Entity : null;
        }

        public async Task<int> UpdateAsync(BrandModel model)
        {
            model.DTsCreate = DateTime.Now;
            _context.BrandModels.Update(model);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(long id)
        {
            var model = await _context.BrandModels.Where(x => x.ModelID == id).Include(x => x.ModelYears)
                .FirstOrDefaultAsync();
            _context.BrandModels.Remove(model);
            return await _context.SaveChangesAsync();
        }
    }
}
