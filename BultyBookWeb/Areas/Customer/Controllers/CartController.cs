using BultyBook.DataAccess.Repository.IRepository;
using BultyBook.Models.VIewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BultyBook.Models;
using BultyBook.Utility;

namespace BultyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                CartList = _unitOfWork.ShoppingCart.GetAll(
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
                CartList = _unitOfWork.ShoppingCart.GetAll(
                    x => x.ApplicationUserId == claim.Value,
                    includeProperies: "Product"),
                OrderHeader = new OrderHeader(),
            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
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

            ShoppingCartVMPOST.CartList = _unitOfWork.ShoppingCart.GetAll(
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

            _unitOfWork.OrderHeader.Add(ShoppingCartVMPOST.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in ShoppingCartVMPOST.CartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVMPOST.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,
                };

                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVMPOST.CartList);
            _unitOfWork.Save();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Add(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);
            if (cart.Count <= 1)
                return Delete(cartId);
            _unitOfWork.ShoppingCart.DecrementCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
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
