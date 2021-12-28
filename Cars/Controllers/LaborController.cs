using Cars.Models;
using Cars.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Controllers
{
    public class LaborController : Controller
    {
        public LaborService services { get; set; }

        public LaborController(LaborService _services)
        {
            services = _services;
           
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetOrderLines(int currentPage)
        {
            try
            {
                var model = services.getOrderLines(currentPage);
                return View(model);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }

        }

        [HttpGet]
        public IActionResult ChangeOrderLinesTablelength(int length)
        {
            try
            {
                return View("OrderLines", services.getOrderLinesWithChangelength(1, length));
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public IActionResult AddLaborvalues(long OrderDetailsID)
        {
            try
            {
                OrderDetails orderDetails = services.GetOrderDetailsByOrderDetailsID(OrderDetailsID);
                if (orderDetails is null && orderDetails.UsedByUser==null)
                {
                    return View("UsedByUser");
                }
                if (orderDetails is not null && !orderDetails.Labor_Hours.HasValue && !orderDetails.Labor_Value.HasValue)
                {
                    ViewBag.types = services.GetSelectListOrderDetailsType();
                    return View(orderDetails);
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
