using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WondyRestaurant.Data;
using WondyRestaurant.Models;
using WondyRestaurant.Models.SubCategoryViewModels;
using WondyRestaurant.Utility;

namespace WondyRestaurant.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class SubcategoriesController : Controller
    {
        public readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }


        public SubcategoriesController(ApplicationDbContext db)
        {
            _db = db;

        }
        public async Task<IActionResult>Index()
        {
            var subCategories = await _db.SubCategory
                .Include(m => m.Category).ToListAsync();
            return View(subCategories);
        }
        public IActionResult Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel
            {
                CategoryList = _db.Category.ToList(),
                SubCategory = new SubCategory(),                
                SubCategoryList = _db.SubCategory.OrderBy(m => m.Name)
                .Select(m => m.Name).ToList()
            };
           

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategory.Where(s => s.Name == model.SubCategory.Name).Count();
                var doesSubCatAndCatExists = _db.SubCategory.Where(s => s.Name == model.SubCategory.Name && s.CategoryId == model.SubCategory.CategoryId).Count();


                if (doesSubCategoryExists > 0 && model.IsNew)
                {
                    //error
                    StatusMessage = "Error : Sub Category Name already Exists";
                }
                else
                {
                    if (doesSubCategoryExists == 0 && !model.IsNew)
                    {
                        //error 
                        StatusMessage = "Error : Sub Category does not exists";
                    }
                    else
                    {
                        if (doesSubCatAndCatExists > 0)
                        {
                            //error
                            StatusMessage = "Error : Category and Sub Cateogry combination exists";
                        }
                        else
                        {
                            _db.Add(model.SubCategory);
                            await _db.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }

            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _db.Category.ToList(),
                SubCategory = model.SubCategory,
                SubCategoryList = _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToList(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);

        }

        // Get Edit
        public async Task<IActionResult>Edit(int? id)
        {
            if (id == null )
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync((System.Linq.Expressions.Expression<Func<SubCategory, bool>>)(m => m.Id == id));
            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _db.Category.ToList(),
                SubCategory = subCategory,
                SubCategoryList = _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToList(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategory.Where(p => p.Name == model.SubCategory.Name).Count();
                var doesSubCatAndCatExists = _db.SubCategory.Where(p => p.Name == model.SubCategory.Name && p.CategoryId == model.SubCategory.Id).Count();

                if (doesSubCategoryExists == 0)
                {
                    StatusMessage = "Error: Sub Category does not exist you cannot add new category here ";
                }
                else
                {
                    if (doesSubCatAndCatExists > 0)
                    {
                        StatusMessage = "Error : Category and Subcategory combinasion arady exist";
                    }
                    else
                    {
                        var subCatFromDb = _db.SubCategory.Find(id);
                        subCatFromDb.Name = model.SubCategory.Name;
                        subCatFromDb.CategoryId = model.SubCategory.CategoryId;
                        await _db.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }

                }
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _db.Category.ToList(),
                SubCategory = model.SubCategory,
                SubCategoryList = _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToList(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);

            
        }
        public  async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }

            var mySubCatergory = await _db.SubCategory
                .Include(s => s.Category)
                .SingleOrDefaultAsync(p => p.Id == id);
            if (mySubCatergory == null)
            {
                return NotFound();

            }
            return View(mySubCatergory);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }

            var mySubCatergory = await _db.SubCategory
                .Include(s => s.Category)
                .SingleOrDefaultAsync(p => p.Id == id);
            if (mySubCatergory == null)
            {
                return NotFound();

            }
            return View(mySubCatergory);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult>DeleteConfirm(int id)
        {
            var findCategory = await _db.SubCategory.SingleOrDefaultAsync((System.Linq.Expressions.Expression<Func<SubCategory, bool>>)(m => m.Id == id));

            _db.SubCategory.Remove((SubCategory)findCategory);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}