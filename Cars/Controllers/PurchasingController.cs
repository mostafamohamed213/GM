using Cars.Service;
using Cars.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Controllers
{
    public class PurchasingController : Controller
    {
        private PurchasingService  service { get; set; }
        public PurchasingController(PurchasingService _service)
        {
            service = _service;
        }
        public IActionResult Index(int currentPage)
        {
            try
            {             
                return View(service.getOrderDetails(currentPage));
            }
            catch (Exception)
            {
                return View("_CustomError");
            }

        }
        [HttpGet]
        public IActionResult ChangeOrderDetailsTablelength(int length)
        {
            try
            {
                return View("Index", service.getOrdersWithChangelength(1, length));
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        [HttpGet]
        public IActionResult AssignVendor(long orderDetailsID)
        {
            try
            {
                var model = service.CloseOrderDetails(orderDetailsID);
                if (model is not null)
                {
                    ViewBag.Runners = service.getRunners();
                    return View("AssignVendor", model);
                }                
               return View("UsedByUser");
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        [HttpPost]
        public IActionResult AssignVendor(long OrderDetailsID,int RunnerID)
        {
            try
            {
                if (OrderDetailsID > 0)
                {
                    if (RunnerID <= 0)
                    {
                       ViewBag.Runners = service.getRunners();
                       ModelState.AddModelError("RunnerID", "This field is required");
                       return View("AssignVendor", service.getOrderDetailsByID(OrderDetailsID));                      
                    }
                 
                    int status = service.AssignVendor( OrderDetailsID,RunnerID);
                    if (status == 1)
                    {
                        service.OpenOrderDetails(OrderDetailsID);
                        return RedirectToAction("Index", "Purchasing" ,new {currentPage = 1});
                    }
                }
                return View("_CustomError");

            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public IActionResult OpenOrderDetails(long OrderDetailsID)
        {
            try
            {
                service.OpenOrderDetails(OrderDetailsID);
                return RedirectToAction("Index", "Purchasing", new { currentPage = 1 });
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        [HttpGet]
        public IActionResult CancelOrderDetails(long OrderDetailsID)
        {
            try
            {
                var orderDetails = service.CloseOrderDetails(OrderDetailsID);
                if (orderDetails is not null)
                {
                    CancelOrderDetailsViewModel model = new CancelOrderDetailsViewModel()
                    {
                        OrderDetails = orderDetails,
                        OrderDetailsID = orderDetails.OrderDetailsID
                    };
                    return View("CancelOrderDetails", model);
                }
                return View("UsedByUser");
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        [HttpPost]
        public IActionResult CancelOrderDetails(CancelOrderDetailsViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                   int status = service.CancelOrderDetails(model);
                    if (status ==1)
                    {
                        return RedirectToAction("Index", "Purchasing", new { currentPage = 1 });
                    }
                    return View("_CustomError");
                }
                var orderDetails = service.getOrderDetailsByID(model.OrderDetailsID);
                CancelOrderDetailsViewModel model1 = new CancelOrderDetailsViewModel()
                {
                    OrderDetails = orderDetails,
                    OrderDetailsID = orderDetails.OrderDetailsID
                };
                return View("CancelOrderDetails", model1);
              
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
    }
}
