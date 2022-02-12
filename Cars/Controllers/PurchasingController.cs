using Cars.Models;
using Cars.Service;
using Cars.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Cars.Controllers
{
    [Authorize(Permissions.Purchasing.Manage)]
    public class PurchasingController : Controller
    {
        private PurchasingService  service { get; set; }
        //private readonly UserManager<ApplicationUser> userManager;
        //private readonly RoleManager<IdentityRole> roleManager;
        public PurchasingController(PurchasingService _service/*, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager*/)
        {
            service = _service;
            //this.userManager = userManager;
            //this.roleManager = roleManager;
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
        public IActionResult SearchOrderLines(string search)
        {

            try
            {
                if (string.IsNullOrEmpty(search))
                {
                    return RedirectToAction("Index", "Purchasing", new { currentPage = 1 });
                }
                else
                {
                    ViewData["CurrentFilter"] = search;
                    return View("Index", service.SearchOrderLines(search));
                }

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
                var model = service.CloseOrderDetails(orderDetailsID, User.FindFirstValue(ClaimTypes.NameIdentifier));
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
        public IActionResult AssignVendor(long OrderDetailsID,string RunnerID)
        {
            try
            {
                if (OrderDetailsID > 0)
                {
                    if (string.IsNullOrEmpty(RunnerID))
                    {
                       ViewBag.Runners = service.getRunners();
                       ModelState.AddModelError("RunnerID", "This field is required");
                       return View("AssignVendor", service.getOrderDetailsByID(OrderDetailsID));                      
                    }
                 
                   int status = service.AssignVendor( OrderDetailsID, RunnerID, User.FindFirstValue(ClaimTypes.NameIdentifier));
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
                var orderDetails = service.CloseOrderDetails(OrderDetailsID, User.FindFirstValue(ClaimTypes.NameIdentifier));
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
                   int status = service.CancelOrderDetails(model, User.FindFirstValue(ClaimTypes.NameIdentifier));
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
        //[NonAction]
        //public void GetRunners()
        //{
        //    var users = (from s in userManager.Users
        //                 where s.Id != "039e233e-da34-4bbc-aa4a-8b5ff8942e48"
        //                 select s).OrderBy(a => a.UserName).ToList();
        //    var role= roleManager.Roles.Where(a => a.Name != "Runner" || a.Name != "Runners").FirstOrDefault();
        //    var roles = roleManager.user.Where(a => a.Name != "Runner" || a.Name != "Runners").FirstOrDefault();
        //    var runners =userManager.User.Where(c=>c.).
        //}
    }
}
