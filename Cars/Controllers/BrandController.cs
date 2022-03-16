using Cars.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Controllers
{
    public class BrandController : Controller
    {
        private BrandService _service;
        public BrandController(BrandService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(int currentPage, string search)
        {
            try
            {
                ViewData["CurrentFilter"] = search;
                currentPage = (currentPage <= 0) ? 1 : currentPage;
                var result = await _service.GetAllAsync(currentPage, search);
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
                var result = await _service.GetAllWithChangelengthAsync(1, length);

                return View("Index", result);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Models.Brand model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null)
                        return View("_CustomError");

                    var result = await _service.AddAsync(model);
                    if (result == null)
                        return View("_CustomError");
                    else
                        return RedirectToAction(nameof(Index));
                }
                else
                    return View(model);
            }
            catch (Exception)
            {

                return View("_CustomError");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                if (id <= 0)
                    return View("_CustomError");

                var orderLine = await _service.GetByIDAsync(id);
                return View(orderLine);
            }
            catch (Exception)
            {

                return View("_CustomError");
            }
        }

        public async Task<IActionResult> Edit(Models.Brand model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null || model.BrandID <= 0)
                        return View("_CustomError");

                    var result = await _service.UpdateAsync(model);
                    if (result <= 0)
                        return View("_CustomError");
                    else
                        return RedirectToAction(nameof(Index));
                }
                else
                    return View(model);
            }
            catch (Exception)
            {

                return View("_CustomError");
            }
        }

        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                if (id <= 0)
                    return View("_CustomError");

                var result = await _service.DeleteAsync(id);
                if (result <= 0)
                    return View("_CustomError");
                else
                    return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                return View("_CustomError");
            }
        }
    }
}