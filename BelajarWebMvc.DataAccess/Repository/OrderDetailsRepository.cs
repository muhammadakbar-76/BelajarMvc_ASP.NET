using BelajarMvcWeb.DataAccess.Repository.IRepository;
using BelajarMvcWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelajarMvcWeb.DataAccess.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        private DataContext _db;

        public OrderDetailsRepository(DataContext context): base(context)
        {
            _db = context;
        }

        public void Update(OrderDetails orderDetails)
        {
            _db.OrderDetails.Update(orderDetails);
        }
    }
}
