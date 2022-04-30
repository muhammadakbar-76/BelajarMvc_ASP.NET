using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelajarMvcWeb.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }

        ICoverTypeRepository CoverType { get; }

        IProductRepository Product { get; }

        ICompanyRepository Company { get; }

        IApplicationUserRepository ApplicationUser { get; }

        IShoppingCartRepository ShoppingCart { get; }

        IOrderDetailsRepository OrderDetails { get; }
        
        IOrderHeadersRepository OrderHeaders { get; }

        void Save();
    }
}
