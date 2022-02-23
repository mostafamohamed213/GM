using Cars.Models;
using Cars.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Cars.Controllers
{
    [Authorize(Permissions.HR.Manage)]
    public class UserBranchController : Controller
    {
        private readonly UserBranchService _service;
        private readonly BranchService _branchService;

        public UserBranchController(UserBranchService service, BranchService branchService)
        {
            _service = service;
            _branchService = branchService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string currentFilter, string searchString, int? pageNumber)
        {
            try
            {
                var result = await _service.GetAllAsync(currentFilter, searchString, pageNumber, 10);
                ViewData["CurrentFilter"] = searchString;
                ViewBag.pages = (result != null) ? result.TotalPages : 1;
                ViewBag.currentpage = (result != null) ? result.PageIndex : 1;
                return View(result);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string userid, int? userbranchid)
        {
            try
            {
                if (userid == null)
                {
                    return View("_CustomError");
                }

                var userBranchModel = await _service.GetByIDAsync(userid, userbranchid);
                if (userBranchModel == null)
                {
                    return View("_CustomError");
                }

                return View(userBranchModel);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string userid, int? userbranchid)
        {
            try
            {
                var userBranch = await _service.GetByIDAsync(userid, userbranchid);
                if (userBranch == null)
                    return View("_CustomError");

                //Get Active Branches Dropdown
                var branches = await _branchService.GetAllActiveAsync();
                if (userBranch.UserBranchID <= 0)
                    ViewData["Branches"] = new SelectList(branches, "BranchID", "Name");
                else
                    ViewData["Branches"] = new SelectList(branches, "BranchID", "Name", userBranch.BranchID);
                return View(userBranch);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string userid, int? userbranchid, UserBranchViewModel userBranchModel)
        {
            try
            {
                if (userBranchModel.BranchID <= 0)
                    return View("_CustomError");

                if (userid != userBranchModel.UserId || userbranchid != userBranchModel.UserBranchID)
                    return View("_CustomError");

                if (ModelState.IsValid)
                {
                    if (userbranchid == null || userbranchid <= 0)
                    {
                        var addResult = await _service.AddAsync(userid, userBranchModel.BranchID);
                        if (addResult <= 0)
                            return View("_CustomError");
                        return RedirectToAction(nameof(Index));
                    }
                    //Update all userbranches is active = false , Add new userBranch, or Update is Active for selected branch = true 
                    else
                    {
                        var result = await _service.ChangeUserBranchAsync(userBranchModel);
                        if (result <= 0)
                        {
                            ViewBag.ErrorMessage = "Faild To Update Branch";
                            return View(userBranchModel);
                        }
                        return RedirectToAction(nameof(Index));
                    }
                }

                //Get Active Branches Dropdown
                var branches = await _branchService.GetAllActiveAsync();
                if (userBranchModel.UserBranchID <= 0)
                    ViewData["Branches"] = new SelectList(branches, "BranchID", "Name");
                else
                    ViewData["Branches"] = new SelectList(branches, "BranchID", "Name", userBranchModel.BranchID);
                return View(userBranchModel);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
    }
}
