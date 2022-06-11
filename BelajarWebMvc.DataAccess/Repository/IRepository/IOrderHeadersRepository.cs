using BelajarMvcWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelajarMvcWeb.DataAccess.Repository.IRepository
{
    public interface IOrderHeadersRepository: IRepository<OrderHeaders>
    {
        void Update(OrderHeaders orderHeaders);
        void UpdateStatus(int id, string orderStatus, string? paymentStatus=null);
        void UpdatePayment(int id, string sessionId, string payemntIntentId);
    }
}
