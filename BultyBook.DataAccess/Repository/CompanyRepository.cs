using BultyBook.DataAccess.Repository.IRepository;
using BultyBook.DataAccess.Data;
using BultyBook.Models;

namespace BultyBook.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext context;
        public CompanyRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Update(Company company)
        {
            context.Companies.Update(company);
        }
    }
}
