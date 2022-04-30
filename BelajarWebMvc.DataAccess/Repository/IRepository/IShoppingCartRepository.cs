using BelajarMvcWeb.Models;
using BelajarMvcWeb.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelajarMvcWeb.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository: IRepository<ShoppingCart>
    {
        void Update(ShoppingCart shoppingCart);
    }
}
