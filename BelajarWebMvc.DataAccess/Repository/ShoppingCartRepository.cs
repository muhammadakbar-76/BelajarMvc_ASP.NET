using BelajarMvcWeb.DataAccess.Repository.IRepository;
using BelajarMvcWeb.Models;
using BelajarMvcWeb.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelajarMvcWeb.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private DataContext _db;

        public ShoppingCartRepository(DataContext context): base(context)
        {
            _db = context;
        }

        public void Update(ShoppingCart shoppingCart)
        {
            _db.ShoppingCarts.Update(shoppingCart);
        }
    }
}
