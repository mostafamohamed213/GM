using Cars.Models;
using Cars.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Cars.Controllers
{
    [Authorize(Permissions.Inventory.Manage)]
    public class InventoryController : Controller
    {
        private readonly InventoryService _service;
        private readonly UserService _userService;
        private readonly NotificationService _notificationService;
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public InventoryController(InventoryService service, UserService userService,NotificationService notificationService)
        {
            _service = service;
            _userService = userService;
            _notificationService = notificationService;
        }
        public async Task<IActionResult> Index(int currentPage, string search)
        {
            try
            {
                ViewData["CurrentFilter"] = search;
                currentPage = (currentPage <= 0) ? 1 : currentPage;
                var result = await _service.GetInventoryOrderDetialsAsync(currentPage, search);
                return View(result);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ChangeTablelength(int length)
        {
            try
            {
                var result = await _service.GetInventoryOrderDetialsWithChangelengthAsync(1, length);

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

                var orderLine = await _service.GetInventoryOrderDetialsByIDAsync(orderDetailsID);
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

                var assignUserResult = await _service.AssignOrderDetailsToUserAsync(orderDetailsID, userId);
                if (assignUserResult == null)
                    return View("_CustomError");
                else if (assignUserResult.Status == -1)
                    return View("InventoryUsedByUser");
                else if (assignUserResult.model == null)
                    return View("_CustomError");
                else if (assignUserResult.Status <= 0)
                    return View("_CustomError");

                //If location 
                if (assignUserResult.model.UserBranchID != assignUserResult.model.VendorLocationID)
                {
                    var deliveries = await _userService.GetAllByRoleAsync("delivery");
                    if (deliveries != null && deliveries.Count() > 0)
                        ViewBag.Delivery = new SelectList(deliveries, "ID", "Name", assignUserResult.model.DeliveryID);
                }
                List<SelectListItem> Status = new()
                {
                    new SelectListItem { Value = "3", Text = "Done" },
                    new SelectListItem { Value = "2", Text = "WIP" },
                    new SelectListItem { Value = "6", Text = "Rejected" }
                };
                ViewBag.Status = Status;
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

                var releaseUserResult = await _service.ReleaseOrderDetailsFromUserAsync(orderDetailsID, userId);
                if (releaseUserResult <= 0)
                    return View("_CustomError");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                return View("_CustomError");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Submit(Models.OrderDetails model)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (model.OrderDetailsID <= 0)
                    return View("_CustomError");
                //Rejected
                if (model.StatusID == 6)
                    return View("UploadFiles", model);
                else if (model.StatusID == 2)
                {
                    //Release Order Details And Leave Order And Update Quantity
                    var result = await _service.ReleaseOrderDetailsFromUserAndUpdateQuntityAsync(model.OrderDetailsID, model.Inventory.Quantity, userId);
                    if (result <= 0)
                        return View("_CustomError");
                }
                else if (model.StatusID == 3)
                {
                    //Release Order Details And Move IT to delivery or sales
                    var result = await _service.ReleaseDoneOrderDetailsFromUserAsync(model, userId);
                    if (result <= 0)
                        return View("_CustomError");
                }
                return RedirectToAction(nameof(Index));
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

        [HttpPost]
        public async Task<IActionResult> UploadFiles(IFormFile[] FormFiles, long InventoryID, long orderDetailsID)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                #region Add Rejected OrderDetails Files To DB, Server 
                List<InventoryDocument> filePaths = new List<InventoryDocument>();
                string path = $"wwwroot/Resources/Inventory/{InventoryID}";
                if (!(Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);

                }
                foreach (IFormFile file in FormFiles)
                {
                    var path1 = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/Resources/Inventory/{InventoryID}", file.FileName);
                    var stream = new FileStream(path1, FileMode.Create);
                    file.CopyTo(stream);
                    filePaths.Add(new InventoryDocument()
                    {
                        Comment = file.FileName,
                        InventoryID = InventoryID,
                        Path = $"/Resources/Inventory/{InventoryID}/{file.FileName}",
                        SystemUserID = userId
                    });
                }
                if (filePaths != null && filePaths.Count() > 0)
                    await _service.AddFilesAsync(filePaths); 
                #endregion

                //Reject Order Details, Create new One ,And Add WorkFlow Details 
                var orderDetailsModel = await _service.ReleaseOrderDetailsFromUserAndRejectAsync(orderDetailsID, userId);
                if (orderDetailsModel == null)
                    return View("_CustomError");

                //Add Notification to Pricing Team 
                var users = await _userService.GetByRoleAsync("pricing");
                if (users != null && users.Count() > 0)
                    await _notificationService.SendNotificationAndEmailForRejectedOrderAsnc(users, "Rejected Order", "There is a Rejected order with ID : " + orderDetailsModel.Prefix, orderDetailsModel.Prefix, "Inventory");
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View("_CustomError");
            }
        }
    }
}
