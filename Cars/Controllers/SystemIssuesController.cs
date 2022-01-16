using Cars.Consts;
using Cars.Models;
using Cars.Service;
using Cars.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Controllers
{
    public class SystemIssuesController : Controller
    {
        public SystemIssuesServices services { get; set; }
        public SystemIssuesController(SystemIssuesServices _services)
        {
            services = _services;
        }
        public IActionResult GetOrderLinesUsed(int currentPage)
        {
            try
            {
                 var model=   services.GetOrderLinesUsed(currentPage);
                return View("OrderLinesUsed", model);

            }
            catch (Exception)
            {
                return View("_CustomError");
            }
           
        }
        [HttpGet]
        public IActionResult ChangeOrderLinesUsedTablelength(int length)
        {
            try
            {
                return View("OrderLinesUsed", services.getOrderLinesUsedWithChangelength(1, length));
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
                    return RedirectToAction("GetOrderLinesUsed", "SystemIssues", new { currentPage = 1 });
                }
                else
                {
                    ViewData["CurrentFilter"] = search;
                    return View("OrderLinesUsed", services.SearchOrderLines(search));
                }

            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public IActionResult ViewOrderDetails(long OrderDetailsID)
        {
            try
            {
                OrderDetails orderDetails = services.GetOrderDetailsByOrderDetailsID(OrderDetailsID);               
                if (orderDetails is not null )
                {
                    return View("ViewOrderDetails", orderDetails);
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
            long orderDetails = services.OpenOrderDetails(OrderDetailsID);
            if (orderDetails > 0)
            {
                return RedirectToAction("GetOrderLinesUsed", "SystemIssues",new {currentPage = 1});
            }
            return View("_CustomError");            
        }


    }
}
