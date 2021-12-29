using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cars.Models;
using Cars.Service;

namespace Cars.Controllers
{
    public class BranchController : Controller
    {
        private readonly CarsContext _context;
        private readonly BranchService _service;

        public BranchController(CarsContext context, BranchService service)
        {
            _context = context;
            _service = service;
        }

        // GET: Branch
        public async Task<IActionResult> Index()
        {
            return View(await _context.Branches.ToListAsync());
        }

        // GET: Branch/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branchModel = await _service.GetByIDAsync(id.Value);
            if (branchModel == null)
            {
                return NotFound();
            }

            return View(branchModel);
        }

        // GET: Branch/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Branch/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BranchID,Name,BranchIP")] BranchModel branchModel)
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
                await _service.AddAsync(branchModel);
                return RedirectToAction(nameof(Index));
            }
            return View(branchModel);
        }

        // GET: Branch/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branchModel = await _service.GetByIDAsync(id.Value);
            if (branchModel == null)
            {
                return NotFound();
            }
            return View(branchModel);
        }

        // POST: Branch/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BranchID,Name,BranchIP,IsActive")] BranchModel branchModel)
        {
            if (id != branchModel.BranchID)
            {
                return NotFound();
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
                    return NotFound();
                branchModel.DTsCreate = branch.DTsCreate;


                await _service.UpdateAsync(branchModel);
                return RedirectToAction(nameof(Index));
            }
            return View(branchModel);
        }
    }
}
