using Cars.Consts;
using Cars.Models;
using Cars.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Service
{
    public class VendorLocationService
    {
        public CarsContext db { get; set; }
        public VendorLocationService(CarsContext carsContext)
        {
            db = carsContext;
        }

        public PagingViewModel<VendorLocation> getAllVendors(int currentPage,string? search)
        {
           
          
            List<VendorLocation> allVendors = db.VendorLocations.Where(v => v.Enable).Skip((currentPage - 1) * TablesMaxRows.IndexVendorMaxRows).Take(TablesMaxRows.IndexVendorMaxRows).ToList();
            if (search != null)
            {
                allVendors = db.VendorLocations.Where(r => r.Enable && r.NameEn.Contains(search)).Skip((currentPage - 1) * TablesMaxRows.IndexRunnerMaxRows).Take(TablesMaxRows.IndexRunnerMaxRows).ToList();
            }

            try
            {
                if (allVendors is not null)
                {
                    if (search != null)
                    {
                        var result = paginate(allVendors, currentPage);
                        result.itemsCount = db.VendorLocations.Where(r => r.Enable && r.NameEn.Contains(search)).Skip((currentPage - 1) * TablesMaxRows.IndexRunnerMaxRows).Take(TablesMaxRows.IndexRunnerMaxRows).Count();
                        return result;
                    }

                    return paginate(allVendors, currentPage);
                }
                return paginate(null, currentPage);
            }
            catch (Exception)
            {
                return paginate(null, currentPage);
            }
        }

        public PagingViewModel<VendorLocation> getOrderLinesWithChangelength(int currentPageIndex, int length)
        {
            TablesMaxRows.IndexVendorMaxRows = length;
            return getAllVendors(currentPageIndex,null);
        }
        private PagingViewModel<VendorLocation> paginate(List<VendorLocation> vendors, int currentPage)
        {
            PagingViewModel<VendorLocation> viewModel = new PagingViewModel<VendorLocation>();
            viewModel.items = vendors.ToList();
            var itemsCount = db.VendorLocations.Where(v=>v.Enable).Count();
            double pageCount = (double)(itemsCount / Convert.ToDecimal(TablesMaxRows.IndexVendorMaxRows));
            viewModel.PageCount = (int)Math.Ceiling(pageCount);
            viewModel.CurrentPageIndex = currentPage;
            viewModel.itemsCount = itemsCount;
            viewModel.Tablelength = TablesMaxRows.IndexVendorMaxRows;

            return viewModel;
        }
        public long AddVendorLocation(VendorLocationViewModel model)
        {
            try
            {
                if (model is not null)
                {
                    DateTime now = DateTime.Now;
                    VendorLocation addvendorLocation = new VendorLocation()
                    {
                        SystemUserCreate = model.SystemUserCreate,
                        NameEn = model.NameEn,
                        Description = model.Description,
                        Enable =true
                    };                   
                    db.VendorLocations.Add(addvendorLocation);
                    db.SaveChanges();
                   
                    return addvendorLocation.VendorLocationID;
                }
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }

        }
        public long DeleteVendorLocation(long vendorLocationID)
        {
            try
            {
                VendorLocation vedorLocation = db.VendorLocations.FirstOrDefault(v=>v.VendorLocationID== vendorLocationID && v.Enable);
                if(vedorLocation!=null)
                {
                    var orderlines = db.OrderDetails.Where(or => or.VendorLocationID == vedorLocation.VendorLocationID).FirstOrDefault();
                    if(orderlines!=null)
                    {
                        return -1;
                    }
                    vedorLocation.Enable = false;
                    db.SaveChanges();
                    return vedorLocation.VendorLocationID;
                }
               
                return 0;
            }
            catch (Exception)
            {

                return -1;
            }

        }

        internal long EditVendorLocation(long VendorLocationID,VendorLocation model)
        {
            try
            {
                VendorLocation EditVendorLocation = db.VendorLocations.FirstOrDefault(c => c.VendorLocationID == VendorLocationID && c.Enable);
                if (EditVendorLocation is null)
                {
                    return 0;
                }
                EditVendorLocation.NameEn = model.NameEn;
                EditVendorLocation.Description = model.Description;
                EditVendorLocation.SystemUserUpdate = model.SystemUserUpdate;
                EditVendorLocation.DTsUpdate = DateTime.Now;
                db.SaveChanges();
                return VendorLocationID;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public VendorLocation GetVendorLocationByID(long VendorId)
        {
            try
            {
                VendorLocation vednor = db.VendorLocations.Where(v => v.VendorLocationID == VendorId && v.Enable).FirstOrDefault();
                if (vednor is not null)
                {
                    return vednor;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }

    }
}
