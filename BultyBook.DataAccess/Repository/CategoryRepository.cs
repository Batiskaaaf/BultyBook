using BultyBook.DataAccess.Repository.IRepository;
using BultyBook.DataAccess.Data;
using BultyBook.Models;

namespace BultyBook.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext context;
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Update(Category category)
        {
            context.Category.Update(category);
        }
    }
}
