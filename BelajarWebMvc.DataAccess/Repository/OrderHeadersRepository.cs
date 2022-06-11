using BelajarMvcWeb.DataAccess.Repository.IRepository;
using BelajarMvcWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelajarMvcWeb.DataAccess.Repository
{
    public class OrderHeadersRepository : Repository<OrderHeaders>, IOrderHeadersRepository
    {
        private DataContext _db;

        public OrderHeadersRepository(DataContext context): base(context)
        {
            _db = context;
        }

        public void Update(OrderHeaders orderHeaders)
        {
            _db.OrderHeaders.Update(orderHeaders);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderHeaders = _db.OrderHeaders.FirstOrDefault(o => o.Id == id);
            if(orderHeaders is not null)
            {
                orderHeaders.OrderStatus = orderStatus;
                if(paymentStatus is not null)
                {
                    orderHeaders.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdatePayment(int id, string sessionId, string paymentIntentId)
        {
            var orderHeaders = _db.OrderHeaders.FirstOrDefault(o => o.Id == id);
            orderHeaders.PaymentDate = DateTime.Now;
            orderHeaders.SessionId = sessionId;
            orderHeaders.PaymentIntentId = paymentIntentId;
        }
    }
}
