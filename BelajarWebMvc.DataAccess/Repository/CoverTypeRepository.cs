using BelajarMvcWeb.DataAccess.Repository.IRepository;
using BelajarMvcWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelajarMvcWeb.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private DataContext _db;

        public CoverTypeRepository(DataContext db): base(db)
        {
            _db = db;
        }
        public void Update(CoverType coverType)
        {
            _db.CoverTypes.Update(coverType);
        }
    }
}
