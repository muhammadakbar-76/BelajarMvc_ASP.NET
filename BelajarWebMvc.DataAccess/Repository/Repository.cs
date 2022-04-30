using BelajarMvcWeb.DataAccess.Repository.IRepository;
using BelajarMvcWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BelajarMvcWeb.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DataContext _db;
        public DbSet<T> dbSet;

        public Repository(DataContext context)
        {
            _db = context;
            dbSet = _db.Set<T>();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public IEnumerable<T> GetAll(string? includeProperties, Expression<Func<T, bool>>? filter=null)
        {
            IQueryable<T> query = dbSet;
            if(filter is not null)
            {
            query = query
               .AsNoTracking()
               .Where(filter);
            }
            if (includeProperties is not null)
            {
                foreach(var property in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties)
        {
            IQueryable<T> query = dbSet;
            query = query
                .AsNoTracking()
                .Where(filter);
            if(includeProperties is not null)
            {
                foreach (var property in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }
    }
}
