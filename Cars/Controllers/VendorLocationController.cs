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
        public IActionResult GetAllVendorsLocation(int currentPage,string? search)
        {
           try
            {
                
                var allvendors = services.getAllVendors(currentPage,search);

                if (search != null)
                    TempData["lastsearch"] = search;
                else
                    TempData["lastsearch"] = "";


                TempData["ErrorMessage"] = "";
                return View(allvendors);               
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        public IActionResult ChangeOrderLinesTablelength(int length)
        {
            try
            {
                TempData["ErrorMessage"] = "";
                return View("GetAllVendorsLocation", services.getOrderLinesWithChangelength(1, length));
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

        [HttpGet]
        public IActionResult AddVendorLocation()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpPost]
        public IActionResult AddVendorLocation([FromForm]VendorLocationViewModel model)
        {
            try
            {
                model.SystemUserCreate= User.FindFirstValue(ClaimTypes.NameIdentifier);
                long addVendor = services.AddVendorLocation(model);
                if(addVendor>0)
                {
                    var allvendors = services.getAllVendors(1,null);
                    TempData["ErrorMessage"] = "";
                    return View("GetAllVendorsLocation", allvendors);
                }
                return View("_CustomError");
               
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        public IActionResult DeleteVendorLocation(long VendorID)
        {
           
            try
            {
               
                long deleteVendor = services.DeleteVendorLocation(VendorID);
                if (deleteVendor > 0)
                {
                    var allvendors = services.getAllVendors(1,null);
                    TempData["ErrorMessage"] = "";
                    return View("GetAllVendorsLocation", allvendors);
                    
                }
                if(deleteVendor==-1)
                {
                    var allvendors = services.getAllVendors(1,null);
                    TempData["ErrorMessage"] = "Vendor Cannot deleted remove its orders first";
                    return View("GetAllVendorsLocation", allvendors);
                }

                return View("_CustomError");

            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public IActionResult EditVendorLocation(long VendorID)
        {
            try
            {
                var vendor = services.GetVendorLocationByID(VendorID);
                if(vendor!=null)
                return View(vendor);

                return View("_CustomError");
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

    [HttpPost]
        public IActionResult EditVendorLocation([FromForm] VendorLocation model)
        {
            try
            {
                model.SystemUserUpdate= User.FindFirstValue(ClaimTypes.NameIdentifier);
                long editVendor = services.EditVendorLocation(model.VendorLocationID, model);
                if (editVendor > 0)
                {
                    var allvendors = services.getAllVendors(1,null);
                    TempData["ErrorMessage"] = "";
                    return View("GetAllVendorsLocation", allvendors);
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
