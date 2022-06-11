using BelajarMvcWeb.DataAccess.Repository.IRepository;
using BelajarMvcWeb.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BelajarMvcWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) != null)
                {
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
                else
                {
                    var count = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == claim.Value).Count();
                    HttpContext.Session.SetInt32(SD.SessionCart, count);
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
            }
            HttpContext.Session.Clear();
            return View(0);
        }
    }
}
