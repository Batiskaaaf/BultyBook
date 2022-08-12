using Microsoft.EntityFrameworkCore;
using BultyBook.Models;

namespace BultyBook.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<CoverType> CoverType { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;

    }
}
