using ControllersServices.CategoryService.Interfaces;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using Serilog;
using Utility.Constants;

namespace WebShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = IdentityRoleNames.HeadAdminRole + "," + IdentityRoleNames.AdminRole + "," + IdentityRoleNames.TestAdmin)]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService adminCategoryService)
        {
            _categoryService = adminCategoryService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var categoryVM = await _categoryService.GetCategoryVMAsync();
                return View(categoryVM);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in Category/Index");
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index", "ProductBrowser", new { area = "User" });
            }
        }

        public async Task<IActionResult> Upsert(int? id, int? bindedParentCategory)
        {
            try
            {
                var categoryVM = await _categoryService.GetCategoryVMAsync(id, bindedParentCategory);
                return View(categoryVM);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in Category/Upsert for id={Id}, bindedParentCategory={ParentId}", id, bindedParentCategory);
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index");
            }
        }

        public IActionResult AddSubcategory(string parentCategoryFilter)
        {
            try
            {
                if (string.IsNullOrEmpty(parentCategoryFilter))
                {
                    return RedirectToAction("Upsert", new { bindedParentCategory = 0 });
                }

                if (int.TryParse(parentCategoryFilter, out int filterValueInt))
                {
                    return RedirectToAction("Upsert", new { bindedParentCategory = filterValueInt });
                }

                return RedirectToAction("Upsert");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in Category/AddSubcategory with filter={Filter}", parentCategoryFilter);
                TempData["error"] = "Something went wrong, try again later";
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = IdentityRoleNames.HeadAdminRole + "," + IdentityRoleNames.AdminRole)]
        [HttpPost]
        public async Task<IActionResult> Upsert(CategoryVM VM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _categoryService.UpsertCategoryAsync(VM);
                    TempData["success"] = VM.Category.Id == 0 ? "Product added successfully" : "Product updated successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(VM);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while upserting category with id={Id}", VM?.Category?.Id);
                TempData["error"] = "Something went wrong, try again later";
                return View(VM);
            }
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll(string filter)
        {
            try
            {
                var categories = await _categoryService.GetSubcategoriesOfCateogryAsync(filter);
                return Json(new { data = categories });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in Category/GetAll with filter={Filter}", filter);
                return Json(new { data = new object[0] });
            }
        }

        [Authorize(Roles = IdentityRoleNames.HeadAdminRole + "," + IdentityRoleNames.AdminRole)]
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                await _categoryService.DeleteCategoryWithAllSubcategoriesAsync(id);
                TempData["success"] = "Category deleted successfully";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete category with id={Id}", id);
                TempData["error"] = "There was an error when deleting category";
                return Json(new { success = false });
            }
        }

        #endregion
    }
}
