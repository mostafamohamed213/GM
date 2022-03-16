using Cars.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Controllers
{
    public class BrandModelsController : Controller
    {
        private BrandModelsService _service;
        private BrandService _brandService;
        public BrandModelsController(BrandModelsService service, BrandService brandService)
        {
            _service = service;
            _brandService = brandService;
        }

        public async Task<IActionResult> Index(long brandid, int currentPage, string search)
        {
            try
            {
                var brand = await _brandService.GetByIDAsync(brandid);
                if (brand == null)
                    return View("_CustomError");
                ViewBag.Brand = brand.Name;
                ViewBag.BrandID = brand.BrandID;

                ViewData["CurrentFilter"] = search;
                currentPage = (currentPage <= 0) ? 1 : currentPage;
                var result = await _service.GetAllAsync(brandid, currentPage, search);
                return View(result);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ChangeTablelength(long brandid, int length)
        {
            try
            {
                var brand = await _brandService.GetByIDAsync(brandid);
                if (brand == null)
                    return View("_CustomError");
                ViewBag.Brand = brand.Name;
                ViewBag.BrandID = brand.BrandID;

                var result = await _service.GetAllWithChangelengthAsync(brandid, 1, length);

                return View("Index", result);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddAsync(long brandid)
        {
            var brand = await _brandService.GetByIDAsync(brandid);
            if (brand == null)
                return View("_CustomError");
            ViewBag.Brand = brand.Name;
            ViewBag.BrandID = brand.BrandID;
            var brandmodel = new Models.BrandModel()
            {
                BrandID = brandid
            };
            return View(brandmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Models.BrandModel model)
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
                        return RedirectToAction(nameof(Index), new { brandid = result.BrandID });
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

                var brandModel = await _service.GetByIDAsync(id);
                return View(brandModel);
            }
            catch (Exception)
            {

                return View("_CustomError");
            }
        }

        public async Task<IActionResult> Edit(Models.BrandModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null || model.ModelID <= 0)
                        return View("_CustomError");

                    var result = await _service.UpdateAsync(model);
                    if (result <= 0)
                        return View("_CustomError");
                    else
                        return RedirectToAction(nameof(Index), new { brandid = model.BrandID });
                }
                else
                    return View(model);
            }
            catch (Exception)
            {

                return View("_CustomError");
            }
        }

        public async Task<IActionResult> Delete(long id, long brandid)
        {
            try
            {
                if (id <= 0)
                    return View("_CustomError");

                var result = await _service.DeleteAsync(id);
                if (result <= 0)
                    return View("_CustomError");
                else
                    return RedirectToAction(nameof(Index), new { brandid = brandid });
            }
            catch (Exception)
            {

                return View("_CustomError");
            }
        }
    }
}