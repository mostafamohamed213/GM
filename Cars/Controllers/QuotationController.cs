using Cars.Service;
using Cars.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cars.Controllers
{
    public class QuotationController : Controller
    {
        public QuotationService services { get; set; }
        public QuotationController(QuotationService _services)
        {
            services = _services;
        }
        public IActionResult Index(int currentPage)
        {
            try
            {
                ViewBag.countOrderLines = services.getCountOrderLines();
                return View(services.getQuotations(currentPage=1));
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
          
        }
        [HttpGet]
        public IActionResult ChangeQuotationTablelength(int length)
        {
            try
            {
                return View("Index", services.getQuotationsWithChangelength(1, length));
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        [HttpGet]
        public IActionResult CreateQuotation()
        {
            try
            {               
                return View("CreateQuotation", services.getOrderLines());
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
        [HttpPost]
        public IActionResult CreateQuotation(string orderLinesIdList)
        {
            try
            {
                List<long> ids = JsonSerializer.Deserialize<List<long>>(orderLinesIdList);
                if (ids.Count > 0)
                {
                    CreateQuotationViewModel model = services.CreateQuotation(ids);
                    if (model.status == -2)
                    {
                        string val = "Order Lines -> ";
                        foreach (var item in model.orderDetails)
                        {
                            val += $"01:{item.OrderID}:{item.OrderDetailsID} ~ ";
                        }
                        val += "Not available now";
                        ViewBag.ErrorMessage = val;
                        return View("CreateQuotation", services.getOrderLines());
                    }
                    if (model.status == -1)
                    {                       
                        ViewBag.ErrorMessage = "Something went wrong";
                        return View("CreateQuotation", services.getOrderLines());
                    }
                    if (model.status == 1)
                    {
                        return View("UploadFiles", model);
                    }
                }
               


                ViewBag.ErrorMessage = "Please Select Order Lines";
                return View("CreateQuotation", services.getOrderLines());
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
                    services.CreateFilePath($"/Resources/Quotation/{QuotationId}/{file.FileName}", QuotationId,file.FileName);
                }
                return View("Confirmation", QuotationId);
            }
            catch (Exception)
            {

                return View("_CustomError");
            }   

        }

        [HttpPost]
        public IActionResult Confirmation(long QuotationId)
        {
            try
            {
               int status= services.Confirmation(QuotationId);
                if (status == 1)
                {
                    return RedirectToAction("Index", "Quotation");
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
              var quotation = services.getQuotationByQuotationID(quotationID);
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
    }
}
