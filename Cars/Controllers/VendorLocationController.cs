using Cars.Models;
using Cars.Service;
using Cars.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cars.Controllers
{
    public class VendorLocationController : Controller
    {
        public CarsContext db { get; set; }
        public VendorLocationService services { get; set; }
        public VendorLocationController(VendorLocationService _services, CarsContext carsContext)
        {
            services = _services;
            db = carsContext;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult GetAllVendorsLocation()
        {
           try
            {
                var allvendors = services.getAllVendors();
                if(allvendors!=null)
                return View(allvendors);

                return View("_CustomError");
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public IActionResult GetVendorLocationByID(long VendorID)
        {
            try
            {
                var getvendor = services.GetVendorLocationByID(VendorID);
                if(getvendor !=null)
                return View(getvendor);

                return View("_CustomError");
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpPost]
        public IActionResult AddVendorLocation(VendorLocationViewModel model)
        {
            try
            {
                long addVendor = services.AddVendorLocation(model);
                if(addVendor>0)
                {
                    var vendor = db.VendorLocations.FirstOrDefault(v => v.VendorLocationID == addVendor);

                    vendor.SystemUserCreate= User.FindFirstValue(ClaimTypes.NameIdentifier);
                    db.SaveChanges();
                    return View(addVendor);
                }
                return View("_CustomError");
               
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        [HttpDelete]
        public IActionResult DeleteVendorLocation(long VendorID)
        {
            try
            {
                long deleteVendor = services.DeleteVendorLocation(VendorID);
                if (deleteVendor > 0)
                {
                    return View(deleteVendor);
                }
                return View("_CustomError");

            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpPut]
        public IActionResult EditVendorLocation(long VendorID, VendorLocationViewModel model)
        {
            try
            {
                long editVendor = services.EditVendorLocation(VendorID, model);
                if (editVendor > 0)
                {
                    var vendor = db.VendorLocations.FirstOrDefault(v => v.VendorLocationID == editVendor);

                    vendor.SystemUserUpdate = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    db.SaveChanges();
                    return View(editVendor);
                }
                return View("_CustomError");

            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
    }
}
