using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cars.Models;
using Cars.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Cars.Controllers
{
    [Authorize(Permissions.HR.Manage)]
    public class BranchController : Controller
    {
        private readonly CarsContext _context;
        private readonly BranchService _service;
        private readonly UserManager<ApplicationUser> userManager;
        public BranchController( BranchService service, CarsContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.userManager = userManager;
            _service = service;
        }

        // GET: Branch
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

        // GET: Branch/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return View("_CustomError");
                }

                var branchModel = await _service.GetByIDAsync(id.Value);
                if (branchModel == null)
                {
                    return View("_CustomError");
                }

                return View(branchModel);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        // GET: Branch/Create
     
        public IActionResult Create()
        {
            var users = userManager.Users.Select(s => new
            {
                UserID = s.Id,
                Name = string.Format("{0}", s.UserName)
            }).ToList();


            ViewData["Employee"] = new MultiSelectList(users, "UserID", "Name");


            return View();
        }

        // POST: Branch/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BranchID,Name,BranchIP")] BranchModel branchModel, string[] Employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var branch = await _service.GetAsync(branchModel.Name, branchModel.BranchIP);
                    if (branch != null)
                    {
                        if (branch.Name.ToLower() == branchModel.Name.ToLower())
                            ViewBag.ErrorMessage = "Name Already Exists";
                        else
                            ViewBag.ErrorMessage = "Branch IP Already Exists";
                        return View(branchModel);
                    }
                    await _service.AddAsync(branchModel, Employee);
                    return RedirectToAction(nameof(Index));
                }
                return View(branchModel);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        // GET: Branch/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return View("_CustomError");
                }

                var branchModel = await _service.GetByIDAsync(id.Value);
                if (branchModel == null)
                {
                    return View("_CustomError");
                }


                IEnumerable<string> approverFields = _context.UserBranches.Where(a => a.BranchID == id && a.IsActive==true).Select(a => a.UserID).ToList();
                //ViewBag.ResearchFields = Fields;
                branchModel.employee = approverFields.ToArray();



                var users = userManager.Users.Select(s => new
                {
                    UserID = s.Id,
                    Name = string.Format("{0}", s.UserName)
                }).ToList();


                ViewData["Employee"] = new MultiSelectList(users, "UserID", "Name");

                return View(branchModel);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        // POST: Branch/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BranchID,Name,employee,BranchIP,IsActive")] BranchModel branchModel)
        {
            try
            {
                if (id != branchModel.BranchID)
                {
                    return View("_CustomError");
                }

                if (ModelState.IsValid)
                {
                    var branchExists = await _service.GetAsync(branchModel.Name, branchModel.BranchIP, branchModel.BranchID);
                    if (branchExists != null)
                    {
                        if (branchExists.Name.ToLower() == branchModel.Name.ToLower())
                            ViewBag.ErrorMessage = "Name Already Exists";
                        else
                            ViewBag.ErrorMessage = "Branch IP Already Exists";
                        return View(branchModel);
                    }

                    //Get Data 
                    var branch = await _service.GetByIDAsync(branchModel.BranchID);
                    if (branch == null)
                        return View("_CustomError");
                    branchModel.DTsCreate = branch.DTsCreate;


                    await _service.UpdateAsync(branchModel);
                    return RedirectToAction(nameof(Index));
                }
                return View(branchModel);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }
    }
}
