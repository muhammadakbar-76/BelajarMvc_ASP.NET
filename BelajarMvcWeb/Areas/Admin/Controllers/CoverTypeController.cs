using BelajarMvcWeb.DataAccess.Repository.IRepository;
using BelajarMvcWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BelajarMvcWeb.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<CoverType> objCoverTypeList = _unitOfWork.CoverType.GetAll();
            return View(objCoverTypeList);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType cover)
        {
            var coverName = _unitOfWork.CoverType.GetFirstOrDefault(x => x.Name == cover.Name);
            if (coverName != null) ModelState.AddModelError("Name", "Cover Name already exists");
            if (!ModelState.IsValid) return View(cover); //if there's errors, ModelState will be populated
            _unitOfWork.CoverType.Add(cover);
            _unitOfWork.Save();
            TempData["success"] = "Data created Successfully";
            return RedirectToAction("Index"); //second param is controller/route name
        }

        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0) return NotFound();
            var cover = _unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == Id); // there is single/singleordefault, first/firstordefault
            if (cover == null) return NotFound();
            return View(cover);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType cover)
        {
            var cov = _unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == cover.Id);
            if (cov == null) return NotFound();
            if (!ModelState.IsValid) return View(cover); //if there's errors, ModelState will be populated
            _unitOfWork.CoverType.Update(cover);
            _unitOfWork.Save();
            TempData["success"] = "Data edited Successfully";
            return RedirectToAction("Index"); //second param is controller/route name
        }

        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0) return NotFound();
            var cover = _unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == Id); // there is single/singleordefault, first/firstordefault
            if (cover == null) return NotFound();
            _unitOfWork.CoverType.Remove(cover);
            _unitOfWork.Save();
            TempData["success"] = "Data deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
