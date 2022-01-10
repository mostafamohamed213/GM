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
        public IActionResult GetAllRunners()
        {
            try
            {
                var allRunners = services.getAllRunners();
                if (allRunners != null)
                    return View(allRunners);

                return View("_CustomError");
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

        [HttpPost]
        public IActionResult AddRunner([FromBody]RunnerViewModel model)
        {
            try
            {
                model.SystemUserCreate= User.FindFirstValue(ClaimTypes.NameIdentifier);
                long addRunner = services.AddRunner(model);
                if (addRunner > 0)
                {
                    return View(addRunner);
                }
                return View("_CustomError");

            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        [HttpDelete]
        public IActionResult DeleteRunner(long RunnerID)
        {
            try
            {
                long deleteRunner = services.DeleteRunner(RunnerID);
                if (deleteRunner > 0)
                {
                    return View(deleteRunner);
                }
                return View("_CustomError");

            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpPut]
        public IActionResult EditRunner(long RunnerID,[FromBody] RunnerViewModel model)
        {
            try
            {
                model.SystemUserUpdate= User.FindFirstValue(ClaimTypes.NameIdentifier);
                long editRunner = services.EditRunner(RunnerID, model);
                if (editRunner > 0)
                {
                    return View(editRunner);
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
