using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BultyBook.DataAccess.Data;
using BultyBook.Models;
using BultyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using BultyBook.Utility;

namespace BultyBookWeb.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CoverTypesController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public CoverTypesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: CoverTypes
        public IActionResult Index()
        {
            var coverTypes = unitOfWork.CoverType.GetAll();
            return View(coverTypes);
        }

        //GET: CoverTypes/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var coverType = unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);

            if (coverType == null)
                return NotFound();

            return View(coverType);
        }

        //GET: CoverTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        //POST: CoverTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name")] CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.CoverType.Add(coverType);
                unitOfWork.Save();
                TempData["success"] = "Cover type created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }

        //Get: CoverTypes/Edit5
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var coverType = unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);

            if (coverType == null)
                return NotFound();

            return View(coverType);
        }

        //POST: CoverTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name")] CoverType coverType)
        {
            if(id != coverType.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.CoverType.Update(coverType);
                    unitOfWork.Save();
                    TempData["success"] = "Cover type edited successfuly!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!unitOfWork.CoverType.Exist(coverType.Id))
                        NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }

        //GET: CoverTypes/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var coverType = unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);

            if (coverType == null)
                return NotFound();

            return View(coverType);
        }

        //POST: CoverTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var coverType = unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);

            if (coverType == null)
                return NotFound();

            unitOfWork.CoverType.Remove(coverType);
            unitOfWork.Save();
            TempData["success"] = "Cover type deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
