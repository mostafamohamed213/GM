using Cars.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cars.Controllers
{
    public class ModelYearsController : Controller
    {
        private ModelYearsService _service;
        private BrandModelsService _brandModelsService;
        public ModelYearsController(ModelYearsService service, BrandModelsService brandModelsService)
        {
            _service = service;
            _brandModelsService = brandModelsService;
        }

        public async Task<IActionResult> Index(long modelid, int currentPage, string search)
        {
            try
            {
                var brandModel = await _brandModelsService.GetByIDAsync(modelid);
                if (brandModel == null)
                    return View("_CustomError");
                ViewBag.Brand = brandModel.Brand?.Name;
                ViewBag.Model = brandModel.Name;
                ViewBag.ModelID = brandModel.ModelID;

                ViewData["CurrentFilter"] = search;
                currentPage = (currentPage <= 0) ? 1 : currentPage;
                var result = await _service.GetAllAsync(modelid, currentPage, search);
                return View(result);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ChangeTablelength(long modelid, int length)
        {
            try
            {
                var brandModel = await _brandModelsService.GetByIDAsync(modelid);
                if (brandModel == null)
                    return View("_CustomError");
                ViewBag.Brand = brandModel.Brand?.Name;
                ViewBag.Model = brandModel.Name;
                ViewBag.ModelID = brandModel.ModelID;

                var result = await _service.GetAllWithChangelengthAsync(modelid, 1, length);

                return View("Index", result);
            }
            catch (Exception)
            {
                return View("_CustomError");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddAsync(long modelid)
        {
            var model = await _brandModelsService.GetByIDAsync(modelid);
            if (model == null)
                return View("_CustomError");
            ViewBag.Brand = model.Brand?.Name;
            ViewBag.Model = model.Name;
            ViewBag.ModelID = model.ModelID;
            var modelyear = new Models.ModelYear()
            {
                ModelID = modelid
            };
            return View(modelyear);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Models.ModelYear model)
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
                        return RedirectToAction(nameof(Index), new { modelid = result.ModelID });
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

                var modelYears = await _service.GetByIDAsync(id);
                return View(modelYears);
            }
            catch (Exception)
            {

                return View("_CustomError");
            }
        }

        public async Task<IActionResult> Edit(Models.ModelYear model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model == null || model.ModelYearID <= 0)
                        return View("_CustomError");

                    var result = await _service.UpdateAsync(model);
                    if (result <= 0)
                        return View("_CustomError");
                    else
                        return RedirectToAction(nameof(Index), new { modelid = model.ModelID });
                }
                else
                    return View(model);
            }
            catch (Exception)
            {

                return View("_CustomError");
            }
        }

        public async Task<IActionResult> Delete(long id, long modelid)
        {
            try
            {
                if (id <= 0)
                    return View("_CustomError");

                var result = await _service.DeleteAsync(id);
                if (result <= 0)
                    return View("_CustomError");
                else
                    return RedirectToAction(nameof(Index), new { modelid = modelid });
            }
            catch (Exception)
            {

                return View("_CustomError");
            }
        }
    }
}