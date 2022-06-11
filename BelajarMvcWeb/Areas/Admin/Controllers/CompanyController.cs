using BelajarMvcWeb.DataAccess.Repository.IRepository;
using BelajarMvcWeb.Models;
using BelajarMvcWeb.Models.ViewModels;
using BelajarMvcWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BelajarMvcWeb.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? Id = 0)
        {
            Company company = new();
            if (Id != 0)
            {
               company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == Id);
            }
            //ViewBag.CategoryList = categoryList;
            //ViewData["CoverTypeList"] = coverTypeList;
            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj)
        {
            if (!ModelState.IsValid) return View(obj); //if there's errors, ModelState will be populated
            if (obj.Id == 0)
            {
                _unitOfWork.Company.Add(obj);
                TempData["success"] = "Product Data added successfully";
            }
            else
            {
                _unitOfWork.Company.Update(obj);
                TempData["success"] = "Product Data edited successfully";
            }
            _unitOfWork.Save();
            return RedirectToAction("Index"); //second param is controller/route name
        }

        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0) return NotFound();
            var company = _unitOfWork.Company.GetFirstOrDefault(x => x.Id == Id); // there is single/singleordefault, first/firstordefault
            if (company == null) return NotFound();
            _unitOfWork.Company.Remove(company);
            _unitOfWork.Save();
            TempData["success"] = "Data deleted Successfully";
            return RedirectToAction("Index");
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var companyLists = _unitOfWork.Company.GetAll();
            return Json(new
            {
                data = companyLists,
            });
        }

        [HttpDelete]
        public IActionResult Delete(int Id)
        {
            var company = _unitOfWork.Company.GetFirstOrDefault(x => x.Id == Id);
            if (company is null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Company.Remove(company);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Success" });
        }
        #endregion
    }
}
