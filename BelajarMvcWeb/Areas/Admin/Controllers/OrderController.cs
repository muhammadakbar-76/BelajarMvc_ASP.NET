using BelajarMvcWeb.DataAccess.Repository.IRepository;
using BelajarMvcWeb.Models;
using BelajarMvcWeb.Models.ViewModels;
using BelajarMvcWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BelajarMvcWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int orderId)
        {
            OrderVM orderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeaders.GetFirstOrDefault(o => o.Id == orderId, "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(o => o.OrderId == orderId, "Product"),
            };
            return View(orderVM);
        }
        [ActionName("Details")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details_Pay_Now()
        {
            OrderVM.OrderHeader = _unitOfWork.OrderHeaders.GetFirstOrDefault(o => o.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            OrderVM.OrderDetails = _unitOfWork.OrderDetails.GetAll(u => u.OrderId == OrderVM.OrderHeader.Id, includeProperties: "Product");

            //stripe settings
            var domain = "https://localhost:44371";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                    {
                        "card",
                    },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{domain}/Admin/Order/PaymentConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}",
                CancelUrl = $"{domain}/Admin/Order/Details?orderId={OrderVM.OrderHeader.Id}",
            };

            foreach (var item in OrderVM.OrderDetails)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)item.Price * 100,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title,
                        },

                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.OrderHeaders.UpdatePayment(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult PaymentConfirmation(int orderHeaderId)
        {
            OrderHeaders orderHeaders = _unitOfWork.OrderHeaders.GetFirstOrDefault(o => o.Id == orderHeaderId);
            if (orderHeaders.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeaders.SessionId);
                //check stripe status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeaders.UpdateStatus(orderHeaderId, orderHeaders.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }
            return View(orderHeaderId);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetail()
        {
            OrderHeaders orderHeaders = _unitOfWork.OrderHeaders.GetFirstOrDefault(o => o.Id == OrderVM.OrderHeader.Id);
            if (orderHeaders is null)
            {
                ModelState.AddModelError("Order", "Order Not Found");
                return View(OrderVM);
            }
            orderHeaders.UserName = OrderVM.OrderHeader.UserName;
            orderHeaders.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaders.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaders.City = OrderVM.OrderHeader.City;
            orderHeaders.State = OrderVM.OrderHeader.State;
            orderHeaders.PostalCode = OrderVM.OrderHeader.PostalCode;
            if (OrderVM.OrderHeader.Carrier is not null)
            {
                orderHeaders.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (OrderVM.OrderHeader.TrackingNumber is not null)
            {
                orderHeaders.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeaders.Update(orderHeaders);
            _unitOfWork.Save();
            TempData["success"] = "Order Successfully Updated";
            return RedirectToActionPermanent(nameof(Details), "Order", new { orderId = orderHeaders.Id });
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcessing()
        {
            OrderHeaders orderHeaders = _unitOfWork.OrderHeaders.GetFirstOrDefault(o => o.Id == OrderVM.OrderHeader.Id);
            if (orderHeaders is null)
            {
                ModelState.AddModelError("Order", "Order Not Found");
                return View(OrderVM);
            }
            _unitOfWork.OrderHeaders.UpdateStatus(orderHeaders.Id, SD.StatusInProcess);
            _unitOfWork.Save();
            TempData["success"] = "Order Status Successfully Updated";
            return RedirectToActionPermanent(nameof(Details), "Order", new { orderId = orderHeaders.Id });
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        [ValidateAntiForgeryToken]
        public IActionResult ShipOrder()
        {
            OrderHeaders orderHeaders = _unitOfWork.OrderHeaders.GetFirstOrDefault(o => o.Id == OrderVM.OrderHeader.Id);
            if (orderHeaders is null)
            {
                ModelState.AddModelError("Order", "Order Not Found");
                return View(OrderVM);
            }
            orderHeaders.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeaders.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeaders.OrderStatus = SD.StatusShipped;
            orderHeaders.ShippingDate = DateTime.Now;
            if (orderHeaders.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeaders.PaymentDueDate = DateTime.Now.AddDays(30);
            }
            _unitOfWork.OrderHeaders.Update(orderHeaders);
            _unitOfWork.Save();
            TempData["success"] = "Order Shipped Successfully Updated";
            return RedirectToActionPermanent(nameof(Details), "Order", new { orderId = orderHeaders.Id });
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder()
        {
            OrderHeaders orderHeaders = _unitOfWork.OrderHeaders.GetFirstOrDefault(o => o.Id == OrderVM.OrderHeader.Id);
            if (orderHeaders is null)
            {
                ModelState.AddModelError("Order", "Order Not Found");
                return View(OrderVM);
            }
            if (orderHeaders.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeaders.PaymentIntentId,
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeaders.UpdateStatus(orderHeaders.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            else
            {
                _unitOfWork.OrderHeaders.UpdateStatus(orderHeaders.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["success"] = "Order Cancelled Successfully";
            return RedirectToActionPermanent(nameof(Details), "Order", new { orderId = orderHeaders.Id });
        }

        #region API
        [HttpGet]
        public IActionResult GetAll(string status)
        {

            IEnumerable<OrderHeaders> orderHeaders;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeaders.GetAll(includeProperties: "ApplicationUser");
            } else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.OrderHeaders.GetAll(o => o.ApplicationUserId == claim.Value,includeProperties: "ApplicationUser");
            }

            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(o => o.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }
            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}
