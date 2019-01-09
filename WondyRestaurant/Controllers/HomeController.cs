using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WondyRestaurant.Data;
using WondyRestaurant.Models;
using WondyRestaurant.Models.HomeViewModel;

namespace WondyRestaurant.Controllers
{
    public class HomeController : Controller
    {
        public readonly ApplicationDbContext _db;
        public HomeController(ApplicationDbContext db )
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            IndexViewModel indexViewModel = new IndexViewModel()
            {
                MenuItem = await _db.MenuItem.Include(c => c.Category).Include(s => s.SubCategory).ToListAsync(),

                Category = _db.Category.OrderBy(o => o.DisplayOrder),
                Coupons = _db.Coupons.Where(c => c.IsActive == true)
            };

            return View(indexViewModel);
        }
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var menuFromDb = await _db.MenuItem.Include(c => c.Category)
                .Include(s => s.SubCategory)
                .Where(m => m.Id == id).FirstOrDefaultAsync();

            var shoppingCar = new ShoppingCart()
            {
                MenuItem = menuFromDb,
                MenuItemId = menuFromDb.Id
            };
            return View(shoppingCar);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Details(ShoppingCart cartObject)
        {
            cartObject.Id = 0;
            if (ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)this.User.Identity;

                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                cartObject.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = await _db.ShoppingCart
                    .Where(c => c.ApplicationUserId == cartObject.ApplicationUserId && 
                    c.MenuItemId == cartObject.MenuItemId)
                    .FirstOrDefaultAsync();
                if (cartFromDb == null)
                {

                    _db.ShoppingCart.Add(cartObject);

                }
                else
                {
                    cartFromDb.Count = cartFromDb.Count + cartObject.Count;
                }

                await _db.SaveChangesAsync();

                // add the session to retrieve the number of shopping for the user 

                var shoppingCount = _db.ShoppingCart.Where(s =>s.ApplicationUserId == cartObject.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32("cartCount", shoppingCount);
                                               

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var menuFromDb = await _db.MenuItem.Include(c => c.Category)
                .Include(s => s.SubCategory)
                .Where(m => m.Id ==cartObject.MenuItemId).FirstOrDefaultAsync();

            var shoppingCar = new ShoppingCart()
            {
                MenuItem = menuFromDb,
                MenuItemId = menuFromDb.Id
            };
            return View(shoppingCar);
            }
           
        }

        
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
