using Cars.Models;
using Cars.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cars.Controllers
{
    public class LaborController : Controller
    {
        public CarsContext db { get; set; }
        public LaborService services { get; set; }

        public LaborController(LaborService _services, CarsContext carsContext)
        {
            services = _services;

            db = carsContext;

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
               // orderDetails.UsedByUser = null;
                if (orderDetails.UsedByUser!=null)
                {
                    return View("UsedByUser");
                }
                if (orderDetails is not null && !orderDetails.Labor_Hours.HasValue && !orderDetails.Labor_Value.HasValue)
                {
                    orderDetails.UsedByUser= User.FindFirstValue(ClaimTypes.NameIdentifier);
                    orderDetails.UsedDateTime = DateTime.Now;

                  
                        
                    db.SaveChanges();
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

                long OrderId = services.EditOrderDetailsFromSales(orderDetails.Items, orderDetails.Quantity, orderDetails.OrderDetailsTypeID, orderDetails.IsApproved.HasValue ? orderDetails.IsApproved.Value : false, orderDetails?.Labor_Hours, orderDetails.Labor_Value, orderDetails.OrderDetailsID);
                if (OrderId > 0)
                {
                        WorkflowOrderDetailsLog workflowOrder = new WorkflowOrderDetailsLog()
                        {
                            DTsCreate = DateTime.Now,
                            OrderDetailsID = orderDetails.OrderDetailsID,
                            SystemUserID = User.FindFirstValue(ClaimTypes.NameIdentifier),
                            WorkflowID = 3,
                            Active = true,
                        };
                        db.Add(workflowOrder);
                        db.SaveChanges();
                    
                        long orderDetailsId = services.OpenOrderDetails(orderDetails.OrderDetailsID);
                    if (orderDetailsId > 0)
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
                _orderDetails.Items = orderDetails.Items;
                _orderDetails.Quantity = orderDetails.Quantity;
                _orderDetails.IsApproved = orderDetails.IsApproved;
                _orderDetails.OrderDetailsTypeID = orderDetails.OrderDetailsTypeID;
                ViewBag.types = services.GetSelectListOrderDetailsType();
                _orderDetails.Labor_Hours = orderDetails.Labor_Hours;
                _orderDetails.Labor_Value = orderDetails.Labor_Value;
                _orderDetails.UsedByUser = null;
                _orderDetails.UsedDateTime =null;
                _orderDetails.WorkflowID = 4;

                    WorkflowOrderDetailsLog workflowOrder = new WorkflowOrderDetailsLog()
                    {
                        DTsCreate = DateTime.Now,
                        OrderDetailsID = orderDetails.OrderDetailsID,
                        SystemUserID = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        WorkflowID = 3,
                        Active = true,
                    };
                    db.Add(workflowOrder);
                    db.SaveChanges();
                
                return View(_orderDetails);
            }

        }
    }
}
