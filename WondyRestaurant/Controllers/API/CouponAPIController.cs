using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WondyRestaurant.Data;
using WondyRestaurant.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WondyRestaurant.Controllers.API
{
   
    [Route("api/[controller]")]
    public class CouponAPIController : Controller
    {

        private ApplicationDbContext _db;

        public CouponAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get(double orderTotal, string couponCode = null)
        {
            //Return string will have :E for error and :S for success at the end

            var rtn = "";
            if (couponCode == null)
            {
                rtn = orderTotal + ":E";
                return Ok(rtn);
            }

            var couponFromDb = _db.Coupons.Where(c => c.Name == couponCode).FirstOrDefault();

            if (couponFromDb == null)
            {
                rtn = orderTotal + ":E";
                return Ok(rtn);
            }
            if (couponFromDb.MinimumAmount > orderTotal)
            {
                rtn = orderTotal + ":E";
                return Ok(rtn);
            }

            if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupons.ECouponType.Dollar)
            {
                orderTotal = orderTotal - couponFromDb.Discount;
                rtn = orderTotal + ":S";
                return Ok(rtn);
            }
            else
            {
                if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupons.ECouponType.Porcent)
                {
                    orderTotal = orderTotal - (orderTotal * couponFromDb.Discount / 100);
                    rtn = orderTotal + ":S";
                    return Ok(rtn);
                }
            }
            return Ok();
        }



    }
}
