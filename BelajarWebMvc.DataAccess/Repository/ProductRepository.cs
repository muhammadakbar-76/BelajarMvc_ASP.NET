using BelajarMvcWeb.DataAccess.Repository.IRepository;
using BelajarMvcWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelajarMvcWeb.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private DataContext _db;

        public ProductRepository(DataContext db): base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            var obj = _db.Products.FirstOrDefault(u=>u.Id==product.Id);
            if(obj != null)
            {
                obj.Title = product.Title;
                obj.Description = product.Description;
                obj.ISBN = product.ISBN;
                obj.Author = product.Author;
                obj.Price = product.Price;
                obj.Price50 = product.Price50;
                obj.Price100 = product.Price100;
                obj.CategoryId = product.CategoryId;
                obj.CoverTypeId = product.CoverTypeId;
                if(product.ImageUrl != null)
                {
                    obj.ImageUrl = product.ImageUrl;
                }
                _db.SaveChanges();
            }
        }
    }
}
