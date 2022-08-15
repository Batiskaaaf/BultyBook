using BultyBook.DataAccess.Data;
using BultyBook.DataAccess.Repository.IRepository;

namespace BultyBook.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;

        public ICategoryRepository Category { get; private set; }
        public ICoverTypeRepository CoverType { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IProductRepository Product { get; private set; }


        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
            Category = new CategoryRepository(context);
            CoverType = new CoverTypeRepository(context);
            Product = new ProductRepository(context);
            Company = new CompanyRepository(context);

        }
        public void Save()
        {
            context.SaveChanges();
        }
    }
}
