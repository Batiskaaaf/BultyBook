using BultyBook.DataAccess.Repository.IRepository;
using BultyBook.Models;
using BultyBook.Models.VIewModels;
using BultyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BultyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
	public class OrderController : Controller
	{
        private readonly IUnitOfWork unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
		{
            this.unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}

        public IActionResult Details(int orderId)
        {
            OrderVM = new OrderVM()
            {
                OrderHeader = unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == orderId,
                includeProperies: "ApplicationUser"),
                OrderDetail = unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == orderId,
                includeProperies: "Product")

            };

            return View(OrderVM);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;
            if(User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
                orderHeaders = unitOfWork.OrderHeader.GetAll(includeProperies: "ApplicationUser");
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = unitOfWork.OrderHeader.GetAll(
                    x => x.ApplicationUserId == claim.Value,
                    includeProperies: "ApplicationUser");
            }
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(x => x.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.StatusInProcess);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.StatusApproved);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(x => x.OrderStatus == SD.StatusShipped);
                    break;
                default:
                    break;
            }
            return Json(new { data = orderHeaders });
        }

        #endregion

    }
}

