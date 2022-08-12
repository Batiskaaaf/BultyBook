using BultyBook.DataAccess.Data;
using BultyBook.DataAccess.Repository.IRepository;
using BultyBook.Models;


namespace BultyBook.DataAccess.Repository
{
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext context;

        public CoverTypeRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Update(CoverType coverType)
        {
            context.CoverType.Update(coverType);           
        }
    }
}
