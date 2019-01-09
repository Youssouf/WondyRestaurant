using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WondyRestaurant.Data;
using WondyRestaurant.Models;
using WondyRestaurant.Models.OrderDetailsViewModel;
using WondyRestaurant.Services;
using WondyRestaurant.Utility;

namespace WondyRestaurant.Controllers
{
    public class OrderController : Controller
    {
        public int pageSize = 2;
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;
        public OrderController(ApplicationDbContext db, IEmailSender emailSender )
        {
            _db = db;
            _emailSender = emailSender;

        }

        public async Task<IActionResult> Confirm(int id)
        {
            var claimIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = await _db.OrderHeader.Where(o => o.Id == id && o.UserId == claim.Value).FirstOrDefaultAsync(),
                

                OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == id).ToListAsync()

            };

            var customerEmail = _db.Users.Where(u => u.Id == orderDetailsViewModel.OrderHeader.UserId).FirstOrDefault().Email;
            await _emailSender.SendOrderStatusAsync(customerEmail, orderDetailsViewModel.OrderHeader.Id.ToString(), SD.StatusSubmited);


            return View( orderDetailsViewModel);
        }
        public IActionResult OrderHistory(int productPage =1)
        {
            // var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            // var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // OrderListViewModel orderListVM = new OrderListViewModel()
            // {
            //     Orders = new List<OrderDetailsViewModel>()
            // };

            //// List<OrderDetailsViewModel> orderDetailVM = new List<OrderDetailsViewModel>();

            // List<OrderHeader> OrderHeaderList = _db.OrderHeader.Where(u => u.UserId == claim.Value).OrderByDescending(u => u.OrderDate).ToList();

            // foreach (var item in OrderHeaderList)
            // {

            //     OrderDetailsViewModel individual = new OrderDetailsViewModel
            //     {
            //         OrderHeader = item,
            //         OrderDetails = _db.OrderDetails.Where(u => u.OrderId == item.Id).ToList()
            //     };
            //     orderListVM.Orders.Add(individual);

            // }
            // var count = orderListVM.Orders.Count();

            // orderListVM.Orders = orderListVM.Orders.OrderBy(p => p.OrderHeader.Id)
            //     .Skip((productPage - 1) * pageSize)
            //     .Take(pageSize).ToList();

            // orderListVM.PagingInfo = new PagingInfo()
            // {
            //     CurrentPage = productPage,
            //     ItemsPerPage = pageSize,
            //     TotalItem = count
            // };

            // return View(orderListVM);

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            List<OrderHeader> OrderHeaderList = _db.OrderHeader.Where(u => u.UserId == claim.Value).OrderByDescending(u => u.OrderDate).ToList();

            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = _db.OrderDetails.Where(o => o.OrderId == item.Id).ToList()
                };
                orderListVM.Orders.Add(individual);
            }
            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderBy(p => p.OrderHeader.Id)
                .Skip((productPage - 1) * pageSize)
                .Take(pageSize).ToList();

            orderListVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = pageSize,
                TotalItem = count
            };



            return View(orderListVM);






        }
        [Authorize(Roles = SD.AdminEndUser)]
        public IActionResult ManageOrder()
        {
            List<OrderDetailsViewModel> orderDetailVM = new List<OrderDetailsViewModel>();
            List<OrderHeader> OrderHeaderList = _db.OrderHeader.Where(u => u.Status == SD.StatusSubmited || u.Status == SD.StatusInProcess)
                                                 .OrderByDescending(u => u.PickupTime).ToList();

            foreach (var item in OrderHeaderList)
            {

                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = _db.OrderDetails.Where(u => u.OrderId == item.Id).ToList()
                };
                orderDetailVM.Add(individual);

            }
            return View(orderDetailVM);


        }

        [Authorize(Roles =SD.AdminEndUser)]
        public async Task<IActionResult> OrderPrepare(int orderId )
        {
            OrderHeader orderHeader = _db.OrderHeader.Find(orderId);
            orderHeader.Status = SD.StatusInProcess;

            await _db.SaveChangesAsync();
            return RedirectToAction("ManageOrder", "Order");               

        }

        [Authorize(Roles = SD.AdminEndUser)]

        public async Task<IActionResult> OrderReady(int orderId)
        {
            OrderHeader orderHeader = _db.OrderHeader.Find(orderId);
            orderHeader.Status = SD.StatusInReady;

            await _db.SaveChangesAsync();

            var customerEmail = _db.Users.Where(u => u.Id == orderHeader.UserId).FirstOrDefault().Email;
            await _emailSender.SendOrderStatusAsync(customerEmail, orderHeader.Id.ToString(), SD.StatusInReady);

            return RedirectToAction("ManageOrder", "Order");


        }
        public async Task<IActionResult> OrderCancel(int orderId)
        {
            OrderHeader orderHeader = _db.OrderHeader.Find(orderId);
            orderHeader.Status = SD.StatusCancelled;  
            await _db.SaveChangesAsync();

            var customerEmail = _db.Users.Where(u => u.Id == orderHeader.UserId).FirstOrDefault().Email; 
            await _emailSender.SendOrderStatusAsync(customerEmail, orderHeader.Id.ToString(), SD.StatusCancelled);

            return RedirectToAction("ManageOrder", "Order");


        }
        public async Task<IActionResult>OrderPickUp(string searchEmail, string searchPhone, string searchOrder )
        {                        
            List<OrderDetailsViewModel> OrderDetailsVM = new List<OrderDetailsViewModel>();

            if (searchEmail != null || searchPhone != null || searchOrder != null)
            {
                //filtering the criteria
                var user = new ApplicationUser();
                List<OrderHeader> OrderHeaderList = new List<OrderHeader>();

                if (searchOrder != null)
                {
                    OrderHeaderList = _db.OrderHeader.Where(o => o.Id == Convert.ToInt32(searchOrder)).ToList();
                }
                else
                {
                    if (searchEmail != null)
                    {
                        user = _db.Users.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower())).FirstOrDefault();
                    }
                    else
                    {
                        if (searchPhone != null)
                        {
                            user = _db.Users.Where(u => u.PhoneNumber.ToLower().Contains(searchPhone.ToLower())).FirstOrDefault();
                        }
                    }
                }
                if (user != null || OrderHeaderList.Count > 0)
                {
                    if (OrderHeaderList.Count == 0)
                    {
                        OrderHeaderList = _db.OrderHeader.Where(o => o.UserId == user.Id).OrderByDescending(o => o.OrderDate).ToList();
                    }
                    foreach (OrderHeader item in OrderHeaderList)
                    {
                        OrderDetailsViewModel individual = new OrderDetailsViewModel
                        {
                            OrderHeader = item,
                            OrderDetails = _db.OrderDetails.Where(o => o.OrderId == item.Id).ToList()
                        };
                        OrderDetailsVM.Add(individual);
                    }
                }


            }
            else
            {
                List<OrderHeader> OrderHeaderList = _db.OrderHeader.Where(o => o.Status == SD.StatusInReady)
                    .OrderByDescending(u => u.PickupTime).ToList();

                foreach (OrderHeader item in OrderHeaderList)
                {
                    OrderDetailsViewModel individual = new OrderDetailsViewModel
                    {
                        OrderHeader = item,
                        OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                    };
                    OrderDetailsVM.Add(individual);

                }
            }
            return View(OrderDetailsVM);
        }


        [Authorize(Roles = SD.AdminEndUser)]
        public IActionResult OrderPickupDetails(int orderId)
        {
            OrderDetailsViewModel OrderDetailsVM = new OrderDetailsViewModel
            {
                OrderHeader = _db.OrderHeader.Where(o => o.Id == orderId).FirstOrDefault()
            };
            OrderDetailsVM.OrderHeader.ApplicationUser = _db.Users.Where(u => u.Id == OrderDetailsVM.OrderHeader.UserId).FirstOrDefault();
            OrderDetailsVM.OrderDetails = _db.OrderDetails.Where(o => o.OrderId == OrderDetailsVM.OrderHeader.Id).ToList();

            return View(OrderDetailsVM);
        }

        [Authorize(Roles = SD.AdminEndUser)]
        [HttpPost, ActionName("OrderPickupDetails")]
        public async Task<IActionResult> OrderPickupDetailsPost(int orderId)
        {
            OrderHeader orderHeader = _db.OrderHeader.Find(orderId);
            orderHeader.Status = SD.StatusCompleted;

           await _db.SaveChangesAsync();
            return RedirectToAction("OrderPickUp", "Order");

        }



    }
}
