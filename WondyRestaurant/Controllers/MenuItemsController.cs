using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WondyRestaurant.Data;
using WondyRestaurant.Models;
using WondyRestaurant.Models.MenuItemViewModels;
using WondyRestaurant.Utility;

namespace WondyRestaurant.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class MenuItemsController : Controller
    {
        public readonly ApplicationDbContext _db;
        public readonly IHostingEnvironment _hostingEvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemsController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEvironment = hostingEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                Category = _db.Category.ToList(),
                MenuItem = new MenuItem()
            };

        }
        // Get Menu Items
        public async Task<IActionResult> Index()
        {
            var menuItems = _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory);


            return View( await menuItems.ToListAsync());
        }
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }
        
        public JsonResult GetSubCategory(int CategoryId)
        {
            List<SubCategory> subCategoryList = new List<SubCategory>();

            subCategoryList = (from subCategory in _db.SubCategory
                               where subCategory.CategoryId == CategoryId
                               select subCategory).ToList();

            return Json(new SelectList(subCategoryList, "Id", "Name"));
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {

            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }

            _db.MenuItem.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();

            //Image Being Saved
            string webRootPath = _hostingEvironment.WebRootPath;

            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = _db.MenuItem.Find(MenuItemVM.MenuItem.Id);

            if (files[0] != null && files[0].Length > 0)
            {
                //when user uploads an image
                var uploads = Path.Combine(webRootPath, "images");
                var extension = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

                using (var filestream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                //when user does not upload image
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".png";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
        // Get Edit Menu item 
        public async Task<IActionResult>Edit(int ? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(p => p.SubCategory)
                .SingleOrDefaultAsync(m => m.Id == id);
            MenuItemVM.SubCategory = _db.SubCategory.Where(m => m.CategoryId == MenuItemVM.MenuItem.CategoryId).ToList();
            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();  
                    
            }
            return View(MenuItemVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());



            if (id != MenuItemVM.MenuItem.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    string webRootPath = _hostingEvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;
                    var menuItemFromDb = _db.MenuItem.Where(m => m.Id == MenuItemVM.MenuItem.Id).FirstOrDefault();

                    if (files[0].Length > 0 && files[0] != null)
                    {
                        var uploads = Path.Combine(webRootPath, "images");
                        var extension_new = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."),
                            files[0].FileName.Length - files[0].FileName.LastIndexOf("."));
                        var extension_old = menuItemFromDb.Image.Substring(menuItemFromDb.Image.LastIndexOf("."),
                            menuItemFromDb.Image.Length - menuItemFromDb.Image.LastIndexOf("."));

                        var imagPathOld = Path.Combine(uploads + MenuItemVM.MenuItem.Id + extension_old);
                        var imagPathNew = Path.Combine(uploads + MenuItemVM.MenuItem.Id + extension_new);

                        if (System.IO.File.Exists(imagPathOld))
                        {
                            System.IO.File.Delete(imagPathOld);
                        }

                        using (var filestream = new FileStream(imagPathNew, FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }
                        MenuItemVM.MenuItem.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension_new;
                    }

                    if (MenuItemVM.MenuItem.Image != null)
                    {
                        menuItemFromDb.Image = MenuItemVM.MenuItem.Image;
                    }
                    menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
                    menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
                    menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
                    menuItemFromDb.Spicyness = MenuItemVM.MenuItem.Spicyness;
                    menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
                    menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;
                    await _db.SaveChangesAsync();

                }
                catch (Exception ex)
                {

                    //
                }
                return RedirectToAction(nameof(Index));
            }
            MenuItemVM.SubCategory = _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToList();
            return View(MenuItemVM);
            
        }
        //GET : Details MenuItem
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category)
                .Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        //GET : Delete MenuItem

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category)
                .Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            string webRootPath = _hostingEvironment.WebRootPath;
            MenuItem menuItem = await _db.MenuItem.FindAsync(id);
            if (menuItem != null)
            {
                var uploads = Path.Combine(webRootPath, "images");
                var extension = menuItem.Image.Substring(menuItem.Image.LastIndexOf("."),
                           menuItem.Image.Length - menuItem.Image.LastIndexOf("."));
                var imagePath = Path.Combine(uploads, menuItem.Id + extension);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _db.MenuItem.Remove(menuItem);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}