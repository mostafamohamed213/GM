using Cars.Models;
using Cars.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Controllers
{
    public class AllOrderLinesController : Controller
    {
        public AllOrderLinesService services { get; set; }

        public CarsContext db { get; set; }

        public AllOrderLinesController(AllOrderLinesService _services, CarsContext _db)
        {
            services = _services;
            db = _db;
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

        public IActionResult ChangeOrderLinesTablelength(int length)
        {
            try
            {
                return View("GetOrderLines", services.getOrderLinesWithChangelength(1, length));
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        public IActionResult orderdetails(long OrderDetailsID)
        {

            try
            {
              
                    var orderDetails = services.GetOrderDetailsByOrderDetailsID(OrderDetailsID);
                   
                    if (orderDetails is not null )
                    {
                        ViewBag.types = services.GetSelectListOrderDetailsType();
                        var workflow = db.Workflows.Where(w => w.WorkflowID == orderDetails.WorkflowID).FirstOrDefault();
                        TempData["workflow"] = workflow.Name;
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
