using BultyBook.DataAccess.Repository.IRepository;
using BultyBook.DataAccess.Data;
using BultyBook.Models;

namespace BultyBook.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext context;
        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }

        public void Update(ApplicationUser applicationUser)
        {
            context.ApplicationUsers.Update(applicationUser);
        }
    }
}
