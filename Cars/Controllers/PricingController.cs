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
    public class PricingController : Controller
    {
        public PricingService services { get; set; }
        public OrderLineUsedService usedService { get; set; }
        public PricingController(PricingService _orderServices , OrderLineUsedService _usedService)
        {
            services = _orderServices;
            usedService = _usedService;
        }

        public IActionResult Index(int currentPage = 1)
        {
            try
            {
                return View("Index", services.getOrders(currentPage));
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
                return View("Index", services.getOrdersWithChangelength(1, length));
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        [HttpGet]
        public IActionResult EditOrderDetails(long orderDetailsID)
        {
            try
            {
                var orderDetails = usedService.CloseOrderDetails(orderDetailsID, User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (orderDetails is not null)
                {
                    ViewBag.vendorLocations = services.GetSelectListVendorLocations();
                    orderDetails.Children = services.GetOrderDeatilsChildren(orderDetailsID);
                    return View(orderDetails);
                }
                return View("UsedByUser");
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpPost]
        public IActionResult EditOrderDetails(long orderDetailsID ,string PartNumber,decimal Price,int VendorLocationID ,string Comments)
        {
            try
            {
                if (orderDetailsID <= 0)
                {
                    return View("_CustomError");
                }
                if (string.IsNullOrEmpty(PartNumber) || Price <= 0 || VendorLocationID <= 0)
                {
                    ViewBag.vendorLocations = services.GetSelectListVendorLocations();
                    var orderDetails = usedService.CloseOrderDetails(orderDetailsID, User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (string.IsNullOrEmpty(PartNumber))
                    {
                        ModelState.AddModelError("PartNumber", "This field required");
                    }
                    else
                    {
                        orderDetails.PartNumber = PartNumber;
                    }
                    if (Price <= 0)
                    {
                        ModelState.AddModelError("Price", "This field required");
                    }
                    else
                    {
                        orderDetails.Price = Price;
                    }
                    if (VendorLocationID <= 0)
                    {
                         ModelState.AddModelError("VendorLocationID", "This field required");
                    }
                    else
                    {
                        orderDetails.VendorLocationID = VendorLocationID;
                    }
                    return View(orderDetails);
                }
               
                int status = services.AddPricingField(orderDetailsID,PartNumber,Price,VendorLocationID,Comments);
                if (status > 0 )
                {
                    usedService.OpenOrderDetails(orderDetailsID);
                    return RedirectToAction("Index");
                }
                return View("_CustomError");
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        [HttpGet]
        public IActionResult AddOrderLine(long orderDetailsID)
        {
            try
            {
                var orderDetails = services.GetOrderDetailsByOrderDetailsID(orderDetailsID);
                if (orderDetails is not null && orderDetails.OrderDetailsType.NameEn.Contains("Any") && (orderDetails.Children == null || orderDetails.Children.Count < 4))
                {
                    PricingAddOrderLineViewModel model =  new PricingAddOrderLineViewModel()
                    {
                        Items = orderDetails.Items,
                        OrderDetailsTypes = services.GetSelectListOrderDetailsType(),
                        ParentOrderDetailsId = orderDetails.OrderDetailsID,
                        VendorLocations = services.GetSelectListVendorLocations(), 
                        Quantity =orderDetails.Quantity
                    };                 
                    
                    return View(model);
                }
                return View("_CustomError");
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        [HttpPost]
        public IActionResult AddOrderLine(PricingAddOrderLineViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    OrderDetails parent = services.AddOrderLine(model);
                    if (parent is not null)
                    {
                        return RedirectToAction("EditOrderDetails",new { orderDetailsID  = parent.OrderDetailsID});
                    }
                    var orderDetails1 = services.GetOrderDetailsByOrderDetailsID(model.ParentOrderDetailsId);
                    if (orderDetails1 is not null)
                    {
                        model.Items = orderDetails1.Items;
                        model.OrderDetailsTypes = services.GetSelectListOrderDetailsType();
                        model.ParentOrderDetailsId = orderDetails1.OrderDetailsID;
                        model.VendorLocations = services.GetSelectListVendorLocations();
                        model.Quantity = orderDetails1.Quantity;

                    }
                    ViewBag.ErrorMessage = "Something went wrong!";

                    return View("AddOrderLine", model);
                }
                var orderDetails = services.GetOrderDetailsByOrderDetailsID(model.ParentOrderDetailsId);
                if (orderDetails is not null)
                {
                    model.Items = orderDetails.Items;
                    model.OrderDetailsTypes = services.GetSelectListOrderDetailsType();
                    model.ParentOrderDetailsId = orderDetails.OrderDetailsID;
                    model.VendorLocations = services.GetSelectListVendorLocations();
                    model.Quantity = orderDetails.Quantity;


                }
                else
                {
                    ViewBag.ErrorMessage = "Something went wrong!";
                }
                return View("AddOrderLine", model);
            }
            catch (Exception)
            {

                return View("_CustomError");
            }
                  
        }
    }
}
