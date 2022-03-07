using Cars.Service;
using Cars.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Cars.Models;

namespace Cars.Controllers
{
    [Authorize(Permissions.Sales.Manage)]
    public class QuotationController : Controller
    {
        public QuotationService services { get; set; }
        public QuotationController(QuotationService _services)
        {
            services = _services;
        }
        public IActionResult Index(int currentPage = 1)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //ViewBag.countOrderLines = services.getCountOrderLines(userId);
                return View(services.getQuotations(currentPage, userId));
            }
            catch (Exception )
            {
                return View("_CustomError");
            }
          
        }
        [HttpGet]
        public IActionResult ChangeQuotationTablelength(int length)
        {
            try
            {
                return View("Index", services.getQuotationsWithChangelength(1, length, User.FindFirstValue(ClaimTypes.NameIdentifier)));
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        [HttpGet]
        public IActionResult SearchQuotations(string search)
        {

            try
            {
                long searchId;
                if (string.IsNullOrEmpty(search) && long.TryParse(search,out searchId))
                {
                    return RedirectToAction("Index", "Quotation", new { currentPage = 1 });
                }
                else
                {
                    ViewData["CurrentFilter"] = search;
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    ViewBag.countOrderLines = services.getCountOrderLines(userId);
                    return View("Index", services.SearchQuotations(search, userId));
                }

            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        //[HttpGet]
        //public IActionResult CreateQuotation()
        //{
        //    try
        //    {               
        //        return View("CreateQuotation", services.getOrderLines(User.FindFirstValue(ClaimTypes.NameIdentifier)));
        //    }
        //    catch (Exception)
        //    {
        //        return View("_CustomError");
        //    }
        //}
        [HttpGet]
        public IActionResult CreateQuotation()
        {
            try
            {
                return View("ViewOrders", services.getOrders(User.FindFirstValue(ClaimTypes.NameIdentifier)));
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        [HttpGet]
        public IActionResult CreateQuotationViewOrderDetails(long orderID)
        {
            try
            {
                return View("CreateQuotation", services.getOrderLines(User.FindFirstValue(ClaimTypes.NameIdentifier),orderID));
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        //[HttpPost]
        //public IActionResult CreateQuotation(string orderLinesIdList)
        //{
        //    List<OrderMaintenanceViewModel> ids = JsonSerializer.Deserialize<List<OrderMaintenanceViewModel>>(orderLinesIdList);
        //    return View();
        //}
        [HttpPost]
        public IActionResult CreateQuotation(string orderLinesIdList)
        {
            try
            {
                long orderId = 0;
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                List<OrderMaintenanceViewModel> models = JsonSerializer.Deserialize<List<OrderMaintenanceViewModel>>(orderLinesIdList);
                
                if (models.Count > 0)
                {
                    CreateQuotationViewModel model = services.CreateQuotation(models, userId);
                    orderId = model.OrderId;
                    if (model.status == -2)
                    {
                        string val = "Order Lines -> ";
                        foreach (var item in model.orderDetails)
                        {
                            val += item.Prefix;
                        }
                        val += "Not available now";
                        ViewBag.ErrorMessage = val;
                        return View("CreateQuotation", services.getOrderLines(userId, model.OrderId));
                    }
                    if (model.status == -1)
                    {
                        //ViewBag.ErrorMessage = "Something went wrong";
                        //return View("CreateQuotation", services.getOrderLines(userId));
                        return View("_CustomError");
                    }
                    if (model.status == 1)
                    {
                        return View("UploadFiles", model);
                    }
                }
                ViewBag.ErrorMessage = "Please Select Order Lines";
                return View("CreateQuotation", services.getOrderLines(userId, orderId));
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpPost]
        public IActionResult UploadFiles(IFormFile[] FormFiles,long QuotationId)
        {
            try
            {
                if (FormFiles.Length > 0)
                {
                    string path = $"wwwroot/Resources/Quotation/{QuotationId}";
                    if (!(Directory.Exists(path)))
                    {
                        Directory.CreateDirectory(path);
                    }
                    foreach (IFormFile file in FormFiles)
                    {
                        var path1 = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/Resources/Quotation/{QuotationId}", file.FileName);
                        var stream = new FileStream(path1, FileMode.Create);
                        file.CopyTo(stream);
                        services.CreateFilePath($"/Resources/Quotation/{QuotationId}/{file.FileName}", QuotationId, file.FileName, User.FindFirstValue(ClaimTypes.NameIdentifier));
                    }                   
                }
                return View("Confirmation", QuotationId);
            }
            catch (Exception)
            {

                return View("_CustomError");
            }   

        }
        //[HttpPost]
        public IActionResult ConfirmationQuotation(long QuotationId)
        {
            try
            {
                if (QuotationId >0)
                {
                    return View("Confirmation", QuotationId);
                }
                return View("_CustomError");
            }
            catch (Exception)
            {

                return View("_CustomError");
            }

        }

        //[HttpPost]
        public IActionResult Confirmation(long QuotationId)
        {
            try
            {
               int status= services.Confirmation(QuotationId, User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (status == 1)
                {
                    return RedirectToAction("Display", "Quotation",new { quotationID = QuotationId });
                }
                return View("_CustomError");
            }
            catch (Exception)
            {

                return View("_CustomError");
            }

        }
        [HttpGet]
        public IActionResult Display(long quotationID)
        {
            try
            {
              var quotation = services.getQuotationByQuotationID(quotationID, User.FindFirstValue(ClaimTypes.NameIdentifier));
                return View("Display",quotation);
            }
            catch (Exception)
            {

                return View("_CustomError");
            }

        }
        [HttpGet]
        public FileResult DownloadFile(long quotationDocumentID)
        {
            
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"c:\folder\myfile.ext");
            string fileName = "myfile.ext";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        [HttpGet]
        public IActionResult ViewOrderLines()
        {
            try
            {
                var orderDetails = services.GetReverseOrderLine(User.FindFirstValue(ClaimTypes.NameIdentifier));
                ViewBag.orderDetailsDoNotHaveParent = services.GetReverseOrderLinedoNotHaveParent(User.FindFirstValue(ClaimTypes.NameIdentifier), orderDetails.Select(c=>c.OrderDetailsID).ToList());
                return View("ViewOrderLines", orderDetails);
            }
            catch (Exception)
            {

                return View("_CustomError");
            }

        }
        [HttpGet]
        public IActionResult ViewOrderLineDetails(long orderDetailsID)
        {
            try
            {
                var orderDetails = services.GetOrderDetails(orderDetailsID, User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (orderDetails is not null)
                {
                    return View("ViewOrderLinesDetails", orderDetails);
                }
                return View("_CustomError");

            }
            catch (Exception)
            {

                return View("_CustomError");
            }

        }
        [HttpGet]
        public IActionResult ReverseOrderLine(long OrderDetailsID,string BackToSales, string BackToPricing, string BackTolabor)
        {
            try
            { 
                if (OrderDetailsID > 0 &&( !string.IsNullOrEmpty(BackToSales) || !string.IsNullOrEmpty(BackToPricing) || !string.IsNullOrEmpty(BackTolabor)))
                {
                    services.ReverseOrderLine(OrderDetailsID, BackToSales, BackToPricing, BackTolabor, User.FindFirstValue(ClaimTypes.NameIdentifier));
                    return RedirectToAction("ViewOrderLines", "Quotation");
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
