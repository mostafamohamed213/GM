using Cars.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cars.Controllers
{
    public class RunnerOrdersController : Controller
    {
        private readonly RunnerOrdersService _service;

        public RunnerOrdersController(RunnerOrdersService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(int currentPage, string search)
        {
            try
            {
                var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                ViewData["CurrentFilter"] = search;
                currentPage = (currentPage <= 0) ? 1 : currentPage;
                var result = await _service.GetRunnerOrderDetialsAsync(userID,currentPage, search);
                return View(result);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ChangeTablelength(int length)
        {
            try
            {
                var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var result = await _service.GetRunnerOrderDetialsWithChangelengthAsync(userID, 1, length);

                return View("Index", result);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByID(long orderDetailsID)
        {
            try
            {
                if (orderDetailsID <= 0)
                    return View("_CustomError");
                var userID = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var orderLine = await _service.GetRunnerOrderDetialsByIDAsync(orderDetailsID, userID);
                return View("Details", orderLine);
            }
            catch (Exception)
            {

                return View("_CustomError");
            }
        }
    }
}
