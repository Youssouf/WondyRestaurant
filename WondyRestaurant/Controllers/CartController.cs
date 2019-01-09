using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WondyRestaurant.Data;
using WondyRestaurant.Models;
using WondyRestaurant.Models.OrderDetailsViewModel;
using WondyRestaurant.Utility;

namespace WondyRestaurant.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public OrderDetailsCart DetailsCart { get; set; }

        public CartController(ApplicationDbContext db)
        {
            _db = db;

        }
        public IActionResult Index()
        {
            var detailsCart = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader(),
              //  ListCart = new List<ShoppingCart>()
                
            };
            detailsCart.OrderHeader.OrderTotal = 0;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value);
            if (cart!= null)
            {
                detailsCart.ListCart = cart.ToList();
            }
            foreach (var list in detailsCart.ListCart)
            {
                list.MenuItem = _db.MenuItem.FirstOrDefault(m => m.Id == list.MenuItemId);
                detailsCart.OrderHeader.OrderTotal += list.MenuItem.Price * list.Count;
                if (list.MenuItem.Description.Length > 100)
                {
                    list.MenuItem.Description = list.MenuItem.Description.Substring(0, 99) + "...";
                }


            }
            detailsCart.OrderHeader.PickupTime = DateTime.Now;
            return View(detailsCart);
        }

        [HttpPost, ActionName("Index")]
        [ValidateAntiForgeryToken]        
        public IActionResult IndexPOST()
        {
            // We will find user id here
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            DetailsCart.ListCart = _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value).ToList();

            DetailsCart.OrderHeader.OrderDate = DateTime.Now;
            DetailsCart.OrderHeader.UserId = claim.Value;
            DetailsCart.OrderHeader.Status = SD.StatusSubmited;
            OrderHeader orderHeader = DetailsCart.OrderHeader;
            _db.OrderHeader.Add(orderHeader);
            _db.SaveChanges();

            
            foreach (var item in DetailsCart.ListCart)
            {
                item.MenuItem = _db.MenuItem.FirstOrDefault(m => m.Id == item.MenuItemId);
                OrderDetails orderdetails = new OrderDetails()
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = orderHeader.Id,
                    Description = item.MenuItem.Description,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                _db.OrderDetails.Add(orderdetails);

            }
            _db.ShoppingCart.RemoveRange(DetailsCart.ListCart);
            _db.SaveChanges();
            // update the session

            HttpContext.Session.SetInt32("CartCount", 0);

            return RedirectToAction("Confirm", "Order", new { id = orderHeader.Id });            


        }
       

        public IActionResult Plus(int cartId)
        {
            var userFromdb = _db.ShoppingCart.Where(c => c.Id == cartId).FirstOrDefault();
            userFromdb.Count ++;


            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        

        public IActionResult Minus(int cartId)
        {
            var cartFromdb = _db.ShoppingCart.Where(c => c.Id == cartId).FirstOrDefault();

            if (cartFromdb.Count == 1)
            {
                _db.ShoppingCart.Remove(cartFromdb);

                _db.SaveChanges();

                // update the session

                var cnt = _db.ShoppingCart.Where(c => c.ApplicationUserId == cartFromdb.ApplicationUserId).ToList().Count;

                HttpContext.Session.SetInt32("cartCount", cnt);

            }
            else
            {

                cartFromdb.Count -= 1;
                _db.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }


        
    }
}