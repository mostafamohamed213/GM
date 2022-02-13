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
    [Authorize(Permissions.Labor.Manage)]
    public class LaborController : Controller
    {
        public CarsContext db { get; set; }
        public LaborService services { get; set; }
        public OrderLineUsedService usedService { get; set; }
        private static object Lock = new object();

        public LaborController(LaborService _services, CarsContext carsContext, OrderLineUsedService _usedService)
        {
            services = _services;
            db = carsContext;
            usedService = _usedService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetOrderLines(int currentPage,string? type, decimal? from , decimal? to ,int?vendor)
        {
            try
            {
                
                ViewData["type"] = db.OrderDetailsType.ToList();
                TempData["lastype"] = type;

                ViewData["vendor"] = db.Branches.ToList();
                if(vendor!=null)
                TempData["lastvendor"] = db.Branches.Where(v=>v.BranchID==vendor).Select(v=>v.Name).FirstOrDefault();

                var model = services.getByType(currentPage,type,from ,to,vendor);
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
                ViewData["type"] = db.OrderDetailsType.ToList();
               

                ViewData["vendor"] = db.Branches.ToList();
               
                return View("GetOrderLines", services.getOrderLinesWithChangelength(1, length));
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
                    OrderDetails orderDetails = usedService.CloseOrderDetails(OrderDetailsID, User.FindFirstValue(ClaimTypes.NameIdentifier));
                    //OrderDetails orderDetails = services.GetOrderDetailsByOrderDetailsID(OrderDetailsID);
                    // orderDetails.UsedByUser = null;
                    if (orderDetails == null)
                    {
                        return View("UsedByUser");
                    }
                    if (!orderDetails.Labor_Hours.HasValue && !orderDetails.Labor_Value.HasValue)
                    {
                        //orderDetails.UsedByUser2 = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        //orderDetails.UsedDateTime2 = DateTime.Now;
                        //db.SaveChanges();
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

        [HttpPost]
        public IActionResult AddLaborvalues(OrderDetails orderDetails)
        {
            if (ModelState.IsValid)
            { 

                long OrderId = services.EditOrderDetailsFromSales(orderDetails.Items, orderDetails.Quantity, orderDetails.OrderDetailsTypeID, orderDetails.IsApproved.HasValue ? orderDetails.IsApproved.Value : false, orderDetails?.Labor_Hours, orderDetails.Labor_Value, orderDetails.OrderDetailsID, User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (OrderId > 0)
                {
                        //WorkflowOrderDetailsLog workflowOrder = new WorkflowOrderDetailsLog()
                        //{
                        //    DTsCreate = DateTime.Now,
                        //    OrderDetailsID = orderDetails.OrderDetailsID,
                        //    SystemUserID = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        //    WorkflowID = 1,
                        //    Active = true,
                        //};
                        //db.Add(workflowOrder);
                        //db.SaveChanges();                    
                        //long orderDetailsId = services.OpenOrderDetails(orderDetails.OrderDetailsID);
                    var OrderDetails=usedService.OpenOrderDetails(orderDetails.OrderDetailsID, User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (OrderDetails > 0)
                    {
                        return RedirectToAction("GetOrderLines", new { currentPage = 1 });
                    }
                    return View("_CustomError");
                }
                else
                {
                    return View("_CustomError");
                }
            }
            else
            {
                OrderDetails _orderDetails = services.GetOrderDetailsByOrderDetailsID(orderDetails.OrderDetailsID);
              /*  _orderDetails.Items = orderDetails.Items;
                _orderDetails.Quantity = orderDetails.Quantity;
                _orderDetails.IsApproved = orderDetails.IsApproved;
                _orderDetails.OrderDetailsTypeID = orderDetails.OrderDetailsTypeID;
                ViewBag.types = services.GetSelectListOrderDetailsType();*/
                //_orderDetails.UsedByUser = null;
                //_orderDetails.UsedDateTime =null;                
                return View(_orderDetails);
            }

        }
 
    }
}
