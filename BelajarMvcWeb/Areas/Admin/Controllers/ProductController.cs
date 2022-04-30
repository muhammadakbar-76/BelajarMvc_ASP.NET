using BelajarMvcWeb.DataAccess.Repository.IRepository;
using BelajarMvcWeb.Models;
using BelajarMvcWeb.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BelajarMvcWeb.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? Id = 0)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(
                    u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString(),
                    }
                ),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(
                    u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString(),
                    }
                ),
            };
            if (Id != 0)
            {
               productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == Id);
            }
            //ViewBag.CategoryList = categoryList;
            //ViewData["CoverTypeList"] = coverTypeList;
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (!ModelState.IsValid) return View(obj); //if there's errors, ModelState will be populated
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if(file is not null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\product");
                var extension = Path.GetExtension(file.FileName);

                if(obj.Product.ImageUrl != null)
                {
                    var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                obj.Product.ImageUrl = @$"\images\product\{fileName+extension}";
            }
            if (obj.Product.Id == 0)
            {
                _unitOfWork.Product.Add(obj.Product);
                TempData["success"] = "Product Data added successfully";
            }
            else
            {
                _unitOfWork.Product.Update(obj.Product);
                TempData["success"] = "Product Data edited successfully";
            }
            _unitOfWork.Save();
            return RedirectToAction("Index"); //second param is controller/route name
        }

        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0) return NotFound();
            var product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == Id); // there is single/singleordefault, first/firstordefault
            if (product == null) return NotFound();
            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();
            TempData["success"] = "Data deleted Successfully";
            return RedirectToAction("Index");
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var productLists = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return Json(new
            {
                data = productLists,
            });
        }

        [HttpDelete]
        public IActionResult Delete(int Id)
        {
            var product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == Id);
            string wwwRootPath = _hostEnvironment.WebRootPath;
            if (product is null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
                var oldImagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                } 
            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Success" });
        }
        #endregion
    }
}
