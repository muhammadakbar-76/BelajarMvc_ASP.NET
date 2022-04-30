using BelajarMvcWeb.DataAccess.Repository.IRepository;
using BelajarMvcWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BelajarMvcWeb.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private DataContext _db;
        public CompanyRepository(DataContext db): base(db)
        {
            _db = db;
        }
        public void Update(Company company)
        {
            _db.Companies.Update(company);
        }
    }
}
