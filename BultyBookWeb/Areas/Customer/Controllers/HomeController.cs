using BultyBook.DataAccess.Repository.IRepository;
using BultyBook.Models;
using BultyBook.Models.VIewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

        public IActionResult Details(int id)
        {
            var cartObj = new ShoppingCart()
            {
                Product = unitOfWork.Product.
                    GetFirstOrDefault(x => x.Id == id, includeProperies: "Category,CoverType"),
                Count = 1
            };
            return View(cartObj);
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