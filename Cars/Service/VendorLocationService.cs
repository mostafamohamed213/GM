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

        public List<VendorLocation> getAllVendors()
        {
            List<VendorLocation> allVendors = db.VendorLocations.ToList();
            try
            {
                if (allVendors is not null)
                {
                    return allVendors;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
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
                        DTsCreate = now,
                        NameEn = model.NameEn,
                        Description = model.Description,
                        Enable =true,
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
                VendorLocation vedorLocation = db.VendorLocations.FirstOrDefault(v=>v.VendorLocationID== vendorLocationID);
                if(vedorLocation!=null)
                {
                    db.VendorLocations.Remove(vedorLocation);
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

        internal long EditVendorLocation(long VendorLocationID,VendorLocationViewModel model)
        {
            try
            {
                VendorLocation EditVendorLocation = db.VendorLocations.FirstOrDefault(c => c.VendorLocationID == VendorLocationID);
                if (EditVendorLocation is null)
                {
                    return 0;
                }
                EditVendorLocation.NameEn = model.NameEn;
                EditVendorLocation.Description = model.Description;
                EditVendorLocation.DTsUpdate = DateTime.Now;
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
                VendorLocation vednor = db.VendorLocations.Where(v => v.VendorLocationID == VendorId).FirstOrDefault();
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
