using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BultyBook.DataAccess.Data;
using System.IO;
using BultyBook.Models;
using BultyBook.DataAccess.Repository.IRepository;
using BultyBook.Models.VIewModels;

namespace BultyBookWeb.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        const string RootImgProducts = @"images\products\";

        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment hostEnvironment;
        public ProductsController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            this.unitOfWork = unitOfWork;
            this.hostEnvironment = hostEnvironment;
        }

        // GET: Products
        public IActionResult Index()
        {
            return View();
        }

        //GET: Products/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var coverType = unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);

            if (coverType == null)
                return NotFound();

            return View(coverType);
        }

        //Get: Products/Edit5
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new Product(),
                CategoryList = unitOfWork.Category.GetAll().Select(
                x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                }),
                CoverTypeList = unitOfWork.CoverType.GetAll().Select(
                    x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString(),
                    }),
            };
        
            if (id == null || id == 0)   //create         
                return View(productVM);
            else                         //update
            {
                productVM.Product = unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
                return View(productVM);
            }           
        }

        //POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (!ModelState.IsValid)
                return View(obj);

            var wwwRootPath = hostEnvironment.WebRootPath;

            if (file != null)
            {
                var fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, RootImgProducts);
                var extension = Path.GetExtension(file.FileName);

                if(obj.Product.ImageUrl != null)
                {
                    var oldImgPath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImgPath))
                        System.IO.File.Delete(oldImgPath);            
                }

                using (var fileStream =  new FileStream(Path.Combine(uploads,fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                obj.Product.ImageUrl = @"\" + RootImgProducts + fileName + extension;
            }
            if(obj.Product.Id == 0)
            {
                TempData["success"] = "Product created successfuly!";
                unitOfWork.Product.Add(obj.Product);
            }
            else
            {
                TempData["success"] = "Product updated successfuly!";
                unitOfWork.Product.Update(obj.Product);
            }

            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
            
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = unitOfWork.Product.GetAll(includeProperies:"Category,CoverType");
            return Json(new { data = productList });
        }

        //POST: Products/Delete/5
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var obj = unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);

            if (obj == null)
                return Json(new { success = false , message="Error while deleting"});

            var oldImgPath = Path.Combine(hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImgPath))
                System.IO.File.Delete(oldImgPath);

            unitOfWork.Product.Remove(obj);
            unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful" });
        }
        #endregion

    }
}
