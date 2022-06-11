using BelajarMvcWeb.DataAccess.Repository.IRepository;
using BelajarMvcWeb.Models;
using BelajarMvcWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BelajarMvcWeb.Controllers
{
    [Area("Admin")] //this is unneccesary, but just for sure i like to add it
    [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _unitOfWork.Category.GetAll();
            return View(objCategoryList);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category cat)
        {
            var catName = _unitOfWork.Category.GetFirstOrDefault(x => x.Name == cat.Name);
            if(catName != null) ModelState.AddModelError("Name", "Category Name already exists");
            if (cat.Name == cat.DisplayOrder.ToString()) ModelState.AddModelError("Name", "Name and Display Order can't be the same");
            if (!ModelState.IsValid) return View(cat); //if there's errors, ModelState will be populated
            _unitOfWork.Category.Add(cat);
            _unitOfWork.Save();
            TempData["success"] = "Data created Successfully";
            return RedirectToAction("Index"); //second param is controller/route name
        }

        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0) return NotFound();
            var cat = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == Id); // there is single/singleordefault, first/firstordefault
            if (cat == null) return NotFound();
            return View(cat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category cat)
        {
            if (cat.Name == cat.DisplayOrder.ToString()) ModelState.AddModelError("Name", "Name and Display Order can't be the same");
            var category = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == cat.Id);
            if (category == null) return NotFound();
            if (!ModelState.IsValid) return View(cat); //if there's errors, ModelState will be populated
            _unitOfWork.Category.Update(cat);
            _unitOfWork.Save();
            TempData["success"] = "Data edited Successfully";
            return RedirectToAction("Index"); //second param is controller/route name
        }

        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0) return NotFound();
            var cat = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == Id); // there is single/singleordefault, first/firstordefault
            if (cat == null) return NotFound();
            _unitOfWork.Category.Remove(cat);
            _unitOfWork.Save();
            TempData["success"] = "Data deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
