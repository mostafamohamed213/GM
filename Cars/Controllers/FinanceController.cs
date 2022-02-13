using Cars.Models;
using Cars.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Cars.Controllers
{
    [Authorize(Permissions.Finance.Manage)]
    public class FinanceController : Controller
    {
        private readonly FinanceService _service;
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public FinanceController(FinanceService service )
        {
            _service = service;
        }

        public async Task<IActionResult> Index(int currentPage, string search)
        {
            try
            {
                ViewData["CurrentFilter"] = search;
                currentPage = (currentPage <= 0) ? 1 : currentPage;
                var result = await _service.GetFinanceOrderDetialsAsync(currentPage, search);
                return View(result);
            }
            catch (Exception ex)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ChangeTablelength(int length)
        {
            try
            {
                var result = await _service.GetFinanceOrderDetialsWithChangelengthAsync(1, length);

                return View("Index", result);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetByID(long orderDetailsID)
        {
            try
            {
                if (orderDetailsID <= 0)
                    return View("_CustomError");

                var orderLine = await _service.GetFinanceOrderDetialsByIDAsync(orderDetailsID);
                return View("Details", orderLine);
            }
            catch (Exception)
            {

                return View("_CustomError");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long orderDetailsID)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                if (orderDetailsID <= 0)
                    return View("_CustomError");

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var assignUserResult = await _service.AssignOrderDetailsFinanceToUserAsync(orderDetailsID, userId);
                if (assignUserResult == null || assignUserResult.model == null)
                    return View("_CustomError");
                else if (assignUserResult.Status == -1)
                    return View("FinanceUsedByUser");
                else if (assignUserResult.Status <= 0)
                    return View("_CustomError");

                return View("Edit", assignUserResult.model);
            }
            catch (Exception)
            {

                return View("_CustomError");
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        [HttpGet]
        public async Task<IActionResult> BackToIndex(long orderDetailsID)
        {
            try
            {
                if (orderDetailsID <= 0)
                    return View("_CustomError");

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var releaseUserResult = await _service.ReleaseOrderDetailsFinanceFromUserAsync(orderDetailsID, userId);
                if (releaseUserResult <= 0)
                    return View("_CustomError");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                return View("_CustomError");
            }
        }

        public async Task<IActionResult> Submit(long orderDetailsID)
        {
            try
            {
                if (orderDetailsID <= 0)
                    return View("_CustomError");

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var submitresult = await _service.SubmotOrderLineandMoveToNextWorkflowAsync(orderDetailsID, userId);
                if (submitresult <= 0)
                    return View("_CustomError");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                return View("_CustomError");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(IFormFile[] FormFiles, long financeID, long orderDetailsID)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                List<FinanceDocument> filePaths = new List<FinanceDocument>();
                string path = $"wwwroot/Resources/Finance/{financeID}";
                if (!(Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);

                }
                foreach (IFormFile file in FormFiles)
                {
                    var path1 = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/Resources/Finance/{financeID}", file.FileName);
                    var stream = new FileStream(path1, FileMode.Create);
                    file.CopyTo(stream);
                    filePaths.Add(new FinanceDocument()
                    {
                        Comment = file.FileName,
                        FinanceID = financeID,
                        Path = $"/Resources/Finance/{financeID}/{file.FileName}",
                        SystemUserID = userId
                    });
                }
                if (filePaths != null && filePaths.Count() > 0)
                    await _service.AddFilesAsync(filePaths);

                var orderDetailsModel = await _service.GetFinanceOrderDetialsByIDAsync(orderDetailsID);
                if (orderDetailsModel == null)
                    return View("_CustomError");
                return View("Edit", orderDetailsModel);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
    }
}
