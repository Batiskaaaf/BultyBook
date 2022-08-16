using BultyBook.DataAccess.Repository.IRepository;
using BultyBook.Models;
using BultyBook.Models.VIewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BultyBookWeb.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IUnitOfWork unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            this.logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = unitOfWork.Product.
                GetAll(includeProperies:"Category,CoverType");
            return View(productList);
        }

        public IActionResult Details(int productId)
        {
            var cartObj = new ShoppingCart()
            {
                Product = unitOfWork.Product.
                    GetFirstOrDefault(x => x.Id == productId, includeProperies: "Category,CoverType"),
                Count = 1,
                ProductId = productId
            };
            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            var cartFromDb = unitOfWork.ShoppingCart.GetFirstOrDefault(
                x => x.ApplicationUserId == claim.Value && x.ProductId == shoppingCart.ProductId);

            if (cartFromDb == null)
                unitOfWork.ShoppingCart.Add(shoppingCart);
            else
                unitOfWork.ShoppingCart.IncrementCount(cartFromDb, shoppingCart.Count);
            
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}