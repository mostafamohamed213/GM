using Cars.Models;
using Cars.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cars.Controllers
{
    [Authorize(Permissions.Delivery.Manage) ]
    public class DeliveryController : Controller
    {
        public DeliveryService services { get; set; }

        public CarsContext db { get; set; }
        public DeliveryController(DeliveryService _services, CarsContext _db)
        {
            services = _services;
            db = _db;
        }
        public IActionResult Index()
        {
            return View("GetOrderLines", 1);

        }

        public IActionResult GetOrderLines(int currentPage, string? search)
        {
            try
            {
                if (search != null)
                    TempData["lastsearch"] = search;
                else
                    TempData["lastsearch"] = "";

                var model = services.getOrderLines(currentPage, search, User.FindFirstValue(ClaimTypes.NameIdentifier));
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
                return View("GetOrderLines", services.getOrderLinesWithChangelength(1, length, User.FindFirstValue(ClaimTypes.NameIdentifier)));
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
                if (orderDetails is not null)
                {
                    var reason = db.OrderDetailsStatusLogs.Where(r => r.OrderDetailsID == orderDetails.OrderDetailsID).FirstOrDefault();
                    ViewBag.types = services.GetSelectListOrderDetailsType();
                    var workflow = db.Workflows.Where(w => w.WorkflowID == orderDetails.WorkflowID).FirstOrDefault();
                    var status = db.Status.Where(s => s.StatusID == orderDetails.StatusID).FirstOrDefault();                    
                    TempData["status"] = status.Name;
                    if (reason != null)
                        TempData["Reason"] = reason.Reason;
                    return View(orderDetails);
                }

                return View("_CustomError");
            }
            catch (Exception)
            {

                return View("_CustomError");
            }

        }

        public IActionResult SubmitToDone([FromForm] int OrderDetailsID)
        {
            try
            {
                var order = db.OrderDetails.Where(o => o.OrderDetailsID == OrderDetailsID).FirstOrDefault();
                order.WorkflowID = 8;
                order.StatusID = 3;
                db.SaveChanges();
                return View("ChangeOrderLinesTablelength", 10);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }


    }
}
