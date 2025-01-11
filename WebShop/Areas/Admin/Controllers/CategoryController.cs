using ControllersServices.CategoryService.Interfaces;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using Serilog;

namespace WebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _adminCategoryService;

        public CategoryController(ICategoryService adminCategoryService)
        {
            _adminCategoryService = adminCategoryService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var categoryVM = await _adminCategoryService.GetCategoryVMAsync();
                return View(categoryVM);
            }
            catch (Exception) 
            {
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index", "Home", new { area = "User" });
            }
        }

        public async Task<IActionResult> Upsert(int? id, int? bindedParentCategory)
        {
            try
            {
                var categoryVM = await _adminCategoryService.GetCategoryVMAsync(id, bindedParentCategory);
                return View(categoryVM);
            }
            catch (Exception)
            {
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index");
            }

        }

        public IActionResult AddSubcategoryToSpecyficCategory(string parentCategoryFilter)
        {
            try
            {
                if (string.IsNullOrEmpty(parentCategoryFilter)) // when root category
                {
                    return RedirectToAction("Upsert", new { bindedParentCategory = 0 });
                }

                if (int.TryParse(parentCategoryFilter, out int filterValueInt))
                {
                    return RedirectToAction("Upsert", new { bindedParentCategory = filterValueInt });
                }

                return RedirectToAction("Upsert");
            }
            catch (Exception)
            {
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(CategoryVM VM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _adminCategoryService.UpsertAsync(VM);
                    TempData["success"] = VM.Category.Id == 0 ? "Product added successfully" : "Product updated successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(VM);
                }
            }
            catch (Exception)
            {
                TempData["error"] = "Something went wrong, try again later";
                return View(VM);
            }
        }

        #region API CALLS
        [HttpGet]
        public async  Task<IActionResult> GetAll(string filter)
        {
            var categories = await _adminCategoryService.GetSubcategoriesOfCateogryAsync(filter);
            return Json(new { data = categories });
        }

        [HttpDelete]
        public async  Task<IActionResult> Delete(int? id)
        {
            try
            {
                await _adminCategoryService.DeleteCategoryWithAllSubcategoriesAsync(id);
                TempData["success"] = "Category deleted successfully";
                return Json(new { success = true,});
            }
            catch (Exception ex)
            {
                Log.Error(ex, "failed to delete category");
                TempData["error"] = "There was en error when deleting category";
                return Json(new { success = false});
            }
        }
        #endregion
    }
}
