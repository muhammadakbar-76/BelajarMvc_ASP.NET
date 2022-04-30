using BelajarMvcWeb.DataAccess.Repository.IRepository;
using BelajarMvcWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelajarMvcWeb.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private DataContext _db;

        public CategoryRepository(DataContext context): base(context)
        {
            _db = context;
        }

        public void Update(Category category)
        {
            _db.Categories.Update(category);
        }
    }
}
