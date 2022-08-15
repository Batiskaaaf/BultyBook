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

namespace BultyBookWeb.Controllers
{
    [Area("Admin")]
    public class CompaniesController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CompaniesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: Categories
        public IActionResult Index()
        {
            var Companies = unitOfWork.Company.GetAll();
            return View(Companies);
        }

        public IActionResult Details(int? id)
        {
            if (id == null || id ==0)
            {
                return NotFound();
            }

            var companies = unitOfWork.Company.GetFirstOrDefault(x => x.Id == id);

            if (companies == null)
            {
                return NotFound();
            }

            return View(companies);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,StreetAddress,City,State,PostalCode,PhoneNumber")] Company company)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Company.Add(company);
                unitOfWork.Save();
                TempData["success"] = "Company created successfuly!";
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var company = unitOfWork.Company.GetFirstOrDefault(m => m.Id == id);

            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,StreetAddress,City,State,PostalCode,PhoneNumber")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    unitOfWork.Company.Update(company);
                    unitOfWork.Save();
                    TempData["success"] = "Company edited successfuly!";

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!unitOfWork.Company.Exist(company.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var company = unitOfWork.Company.GetFirstOrDefault(x => x.Id == id);

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var company = unitOfWork.Company.GetFirstOrDefault(x => x.Id == id);

            if (company == null)            
                return NotFound();           
            
            unitOfWork.Company.Remove(company);
            unitOfWork.Save();
            TempData["success"] = "Company deleted successfuly!";
            return RedirectToAction(nameof(Index));
        }
    }
}
