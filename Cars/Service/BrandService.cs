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
    public class BrandService
    {
        private CarsContext _context { get; set; }
        public BrandService(CarsContext carsContext)
        {
            _context = carsContext;
        }

        public async Task<IEnumerable<Brand>> GetAllForDropDownAsync()
        {
            return await _context.Brand.ToListAsync();
        }

        public async Task<PagingViewModel<Brand>> GetAllAsync(int currentPageIndex, string search)
        {
            List<Brand> brands = null;
            int itemsCount = 0;
            if (string.IsNullOrEmpty(search))
            {
                brands = await _context.Brand.Skip((currentPageIndex - 1) * TablesMaxRows.IndexBrandRows).Take(TablesMaxRows.IndexBrandRows).ToListAsync();
                itemsCount = _context.Brand.Count();
            }
            else
            {
                brands = await _context.Brand.Where(c => (c.Name.ToLower().Trim().Contains(search.ToLower().Trim())))
                    .Skip((currentPageIndex - 1) * TablesMaxRows.IndexBrandRows).Take(TablesMaxRows.IndexBrandRows).ToListAsync();
                itemsCount = _context.Brand.Where(c => (c.Name.ToLower().Trim().Contains(search.ToLower().Trim()))).Count();
            }
            PagingViewModel<Brand> viewModel = new PagingViewModel<Brand>();
            viewModel.items = brands.ToList();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexBrandRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPageIndex;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexBrandRows;
            return viewModel;
        }

        public async Task<PagingViewModel<Brand>> GetAllWithChangelengthAsync(int currentPageIndex, int length)
        {
            TablesMaxRows.IndexBrandRows = length;
            return await GetAllAsync(currentPageIndex, "");
        }

        public async Task<Brand> GetByIDAsync(long id)
        {
            return await _context.Brand.Where(x => x.BrandID == id).FirstOrDefaultAsync();
        }

        public async Task<Brand> AddAsync(Brand model)
        {
            model.DTsCreate = DateTime.Now;
            var result = await _context.Brand.AddAsync(model);
            return (await _context.SaveChangesAsync() > 0) ? result.Entity : null;
        }

        public async Task<int> UpdateAsync(Brand model)
        {
            model.DTsCreate = DateTime.Now;
            _context.Brand.Update(model);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(long id)
        {
            var model = await _context.Brand.Where(x => x.BrandID == id).Include(x => x.BrandModels).ThenInclude(x => x.ModelYears)
                .FirstOrDefaultAsync();
            _context.Brand.Remove(model);
            return await _context.SaveChangesAsync();
        }
    }
}