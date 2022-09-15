using BultyBook.DataAccess.Repository.IRepository;
using BultyBook.Models.VIewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BultyBook.Models;
using BultyBook.Utility;
using Stripe.Checkout;

namespace BultyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                CartList = unitOfWork.ShoppingCart.GetAll(
                    u => u.ApplicationUserId == claim.Value,
                    includeProperies: "Product"),
                OrderHeader = new OrderHeader(),
            };


            foreach (var cart in ShoppingCartVM.CartList)
            {
                cart.Price = GetPriceByQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
            };

            return View(ShoppingCartVM);
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                CartList = unitOfWork.ShoppingCart.GetAll(
                    x => x.ApplicationUserId == claim.Value,
                    includeProperies: "Product"),
                OrderHeader = new OrderHeader(),
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = unitOfWork.ApplicationUser
                .GetFirstOrDefault(x => x.Id == claim.Value);
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach(var cart in ShoppingCartVM.CartList)
            {
                cart.Price = GetPriceByQuantity(
                    cart.Count,
                    cart.Product.Price,
                    cart.Product.Price50,
                    cart.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPOST(ShoppingCartVM ShoppingCartVMPOST)
        {
            
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVMPOST.CartList = unitOfWork.ShoppingCart.GetAll(
                x => x.ApplicationUserId == claim.Value,
                includeProperies: "Product");

            ShoppingCartVMPOST.OrderHeader.ApplicationUserId = claim.Value;         
            ShoppingCartVMPOST.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVMPOST.OrderHeader.OrderStatus = SD.StatusPending;
            ShoppingCartVMPOST.OrderHeader.OrderDate = DateTime.Now;
            
            foreach (var cart in ShoppingCartVMPOST.CartList)
            {
                cart.Price = GetPriceByQuantity(
                    cart.Count,
                    cart.Product.Price,
                    cart.Product.Price50,
                    cart.Product.Price100);
                ShoppingCartVMPOST.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }

            ApplicationUser applicationUser = unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == claim.Value);
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
                ShoppingCartVMPOST.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVMPOST.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
			{
                ShoppingCartVMPOST.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVMPOST.OrderHeader.OrderStatus = SD.StatusApproved;
            }


            unitOfWork.OrderHeader.Add(ShoppingCartVMPOST.OrderHeader);
            unitOfWork.Save();

            foreach (var cart in ShoppingCartVMPOST.CartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVMPOST.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,
                };

                unitOfWork.OrderDetail.Add(orderDetail);
                unitOfWork.Save();
            }



            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
                return StripeCheckout(ShoppingCartVMPOST);
            else
                return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVMPOST.OrderHeader.Id });
        }

        private IActionResult StripeCheckout(ShoppingCartVM ShoppingCartVMPOST)
        {
            var domain = SD.Domain;
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> {"card"},
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVMPOST.OrderHeader.Id}",
                CancelUrl = domain + "customer/cart/index",
            };
            foreach (var item in ShoppingCartVMPOST.CartList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartVMPOST.OrderHeader.Id,session.Id,session.PaymentIntentId);
            unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == id);
            if(orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
			{
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    unitOfWork.Save();
                }
            }

            var shoppingCart = unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId);
            HttpContext.Session.Clear();
            unitOfWork.ShoppingCart.RemoveRange(shoppingCart);
            unitOfWork.Save();
            return View(id);
        }
        public IActionResult Add(int cartId)
        {
            var cart = unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);
            unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cart = unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);
            if (cart.Count <= 1)
            {
                unitOfWork.ShoppingCart.Remove(cart);
                var count = unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).Count() - 1;
                HttpContext.Session.SetInt32(SD.SessionCart, count);
            }
            else
                unitOfWork.ShoppingCart.DecrementCount(cart, 1);
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int cartId)
        {
            var cart = unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);
            unitOfWork.ShoppingCart.Remove(cart);
            unitOfWork.Save();
            var count = unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).Count();
            HttpContext.Session.SetInt32(SD.SessionCart, count); 
            return RedirectToAction(nameof(Index));
        }

        private decimal GetPriceByQuantity(double quantity, decimal price, decimal price50, decimal price100)
        {
            switch (quantity)
            {
                case <= 50:
                    return price;
                case <= 100:
                    return price50;
                case > 100:
                    return price100;
            }
            return price;
        }
    }
}
