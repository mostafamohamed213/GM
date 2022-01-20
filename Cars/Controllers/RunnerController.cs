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
    public class RunnerController : Controller
    {

        public CarsContext db { get; set; }
        public RunnerService services { get; set; }
        public RunnerController(RunnerService _services, CarsContext carsContext)
        {
            services = _services;
            db = carsContext;
        }
        public IActionResult Index()
        {
            return View();
        }
       
       
        [HttpGet]
        public IActionResult GetAllRunners(int currentPage,string? search)
        {
            try
            {
                var allRunners = services.getAllRunners(currentPage, search);

                if (search != null)
                    TempData["lastsearch"] = search;
                else
                    TempData["lastsearch"] = "";


                TempData["ErrorMessage"] = "";
                return View(allRunners);
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

                return View("GetAllRunners", services.getOrderLinesWithChangelength(1, length));
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        [HttpGet]
        public IActionResult GetRunnerByID(long RunnerID)
        {
            try
            {
                var getrunner = services.GetRunnernByID(RunnerID);
                if (getrunner != null)
                    return View(getrunner);

                return View("_CustomError");
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        public IActionResult AddRunner()
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
        public IActionResult AddRunner([FromForm]RunnerViewModel model)
        {
            try
            {
                model.SystemUserCreate= User.FindFirstValue(ClaimTypes.NameIdentifier);
                long addRunner = services.AddRunner(model);
                if (addRunner > 0)
                {
                    var allvrunners = services.getAllRunners(1,null);
                    TempData["ErrorMessage"] = "";
                    return View("GetAllRunners", allvrunners);
                }
                return View("_CustomError");

            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        
        public IActionResult DeleteRunner(long RunnerID)
        {
            try
            {
                long deleteRunner = services.DeleteRunner(RunnerID);
                if (deleteRunner > 0)
                {
                    var allrunners = services.getAllRunners(1,null);
                    TempData["ErrorMessage"] = "";
                    return View("GetAllRunners", allrunners);
                }

                if (deleteRunner == -1)
                {
                    var allrunners = services.getAllRunners(1, null);
                    TempData["ErrorMessage"] = "Vendor Cannot deleted remove its orders first";
                    return View("GetAllRunners", allrunners);
                }
                return View("_CustomError");

            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }


        public IActionResult EditRunner(long RunnerID)
        {
            try
            {
                var Runner = services.GetRunnernByID(RunnerID);
                if (Runner != null)
                    return View(Runner);

                return View("_CustomError");
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpPost]
        public IActionResult EditRunner([FromForm] Runner model)
        {
            try
            {
                model.SystemUserUpdate= User.FindFirstValue(ClaimTypes.NameIdentifier);
                long editRunner = services.EditRunner(model.RunnerID, model);
                if (editRunner > 0)
                {
                    var allvrunners = services.getAllRunners(1,null);
                    TempData["ErrorMessage"] = "";
                    return View("GetAllRunners", allvrunners);
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
