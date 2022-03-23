using AutoMapper;
using Cars.Models;
using Cars.Service;
using Cars.ViewModels;
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
        private IMapper _mapper;

        public LaborController(LaborService _services, CarsContext carsContext, OrderLineUsedService _usedService, IMapper mapper)
        {
            services = _services;
            db = carsContext;
            usedService = _usedService;
            _mapper = mapper;
        }

        //Labor 
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetOrderLines(int currentPage, string type, decimal? from, decimal? to, int? vendor)
        {
            try
            {

                ViewData["type"] = db.OrderDetailsType.ToList();
                TempData["lastype"] = type;

                ViewData["vendor"] = db.Branches.ToList();
                if (vendor != null)
                    TempData["lastvendor"] = db.Branches.Where(v => v.BranchID == vendor).Select(v => v.Name).FirstOrDefault();

                var model = services.getByType(currentPage, type, from, to, vendor);
                var returnedViewModel = _mapper.Map<PagingViewModel<LaborOrdersViewModel>>(model);
                return View(returnedViewModel);
            }
            catch (Exception ex)
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

                var model = services.getOrderLinesWithChangelength(1, length);
                var returnedViewModel = _mapper.Map<PagingViewModel<LaborOrdersViewModel>>(model);

                return View("GetOrderLines", returnedViewModel);
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
                    var OrderDetails = usedService.OpenOrderDetails(orderDetails.OrderDetailsID, User.FindFirstValue(ClaimTypes.NameIdentifier));
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

        //Bulk Labor 
        [HttpPost]
        public IActionResult BulkEdit([FromForm] PagingViewModel<LaborOrdersViewModel> model)

        {
            try
            {
                if (model == null || model.items.Count() <= 0)
                    return RedirectToAction("GetOrderLines", new { currentPage = 1 });
                var selectedOrders = model.items.Where(x => x.IsSelected == true);
                if (selectedOrders == null || selectedOrders.Count() <= 0)
                    return RedirectToAction("GetOrderLines", new { currentPage = 1 });

                var ordersDetails = usedService.BulkCloseOrderDetails(selectedOrders.Select(x => x.OrderDetailsID).ToList(), User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();
                if (ordersDetails == null || ordersDetails.Count() <= 0)
                    return View("UsedByUser");
                ViewBag.types = services.GetSelectListOrderDetailsType();
                return View("AddLaborOrdersValues", ordersDetails.ToList());
            }
            catch (Exception ex)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public IActionResult OpenOrdersDetails(List<long> ids)
        {
            try
            {
                var x = ViewData["IDs"];
                if (ids == null || ids.Count() <= 0)
                    return RedirectToAction("GetOrderLines", new { currentPage = 1 });

                var result = usedService.BulkOpenOrderDetails(ids, User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (result > 0)
                    return RedirectToAction("GetOrderLines", new { currentPage = 1 });
                else
                    return View("_CustomError");
            }
            catch (Exception ex)
            {
                return View("_CustomError");
            }
        }

        [HttpPost]
        public IActionResult AddLaborsvalues([FromForm] List<OrderDetails> model)

        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = services.BulkEditAndOpenAndOrdersDetails(model, User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (result > 0)
                        return RedirectToAction("GetOrderLines", new { currentPage = 1 });
                    return View("_CustomError");
                }
                return View("AddLaborOrdersValues", model);

            }
            catch (Exception ex)
            {
                return View("_CustomError");
            }
        }

        //Returned Labor
        [HttpGet]
        public IActionResult OrderLinesReturned()
        {
            try
            {
                var orderDetails = services.GetReturnedOrderLine();
                var returnedViewModel = _mapper.Map<IEnumerable<LaborOrdersViewModel>>(orderDetails);
                return View(returnedViewModel);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }

        }
        [HttpGet]
        public IActionResult EditOrderDetailsReturned(long orderDetailsID)
        {
            try
            {
                List<int> status = new List<int>();
                status.Add(11); // this id from database            
                OrderDetails orderDetails = usedService.CloseOrderDetailsReturned(orderDetailsID, User.FindFirstValue(ClaimTypes.NameIdentifier), status);
                if (orderDetails is not null)
                {
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
        public IActionResult EditOrderDetailsReturned(long OrderDetailsID, decimal Labor_Hours, double Labor_Value)
        {
            try
            {
                if (OrderDetailsID > 0 && Labor_Hours > 0 && Labor_Value > 0)
                {
                    OrderDetails model = services.EditOrderDetailsReturned(OrderDetailsID, Labor_Hours, Labor_Value, User.FindFirstValue(ClaimTypes.NameIdentifier));

                    if (model is not null)
                    {
                        return RedirectToAction("OrderLinesReturned");
                    }
                    return View("_CustomError");
                }
                List<int> status = new List<int>();
                status.Add(11);
                OrderDetails orderDetails = usedService.CloseOrderDetailsReturned(OrderDetailsID, User.FindFirstValue(ClaimTypes.NameIdentifier), status);
                if (orderDetails is not null)
                {
                    return View(orderDetails);
                }
                return View("UsedByUser");
            }
            catch (Exception)
            {
                return View("_CustomError");
            }

        }

        //Bulk Returned Labor
        [HttpPost]
        public IActionResult BulkEditReturned([FromForm] List<LaborOrdersViewModel> model)

        {
            try
            {
                if (model == null || model.Count() <= 0)
                    return RedirectToAction("OrderLinesReturned");
                var selectedOrders = model.Where(x => x.IsSelected == true);
                if (selectedOrders == null || selectedOrders.Count() <= 0)
                    return RedirectToAction("OrderLinesReturned");

                var ordersDetails = usedService.BulkCloseOrderDetails(selectedOrders.Select(x => x.OrderDetailsID).ToList(), User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();
                if (ordersDetails == null || ordersDetails.Count() <= 0)
                    return View("UsedByUser");
                return View("EditOrdersDetailsReturned", ordersDetails.ToList());
            }
            catch (Exception ex)
            {
                return View("_CustomError");
            }
        }

        [HttpPost]
        public IActionResult EditOrdersDetailsReturned([FromForm] List<OrderDetails> model)

        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = services.BulkEditOrderDetailsReturned(model, User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (result > 0)
                        return RedirectToAction("OrderLinesReturned");
                    return View("_CustomError");
                }
                return View("EditOrdersDetailsReturned", model);

            }
            catch (Exception ex)
            {
                return View("_CustomError");
            }
        }

    }
}