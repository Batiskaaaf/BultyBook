using Microsoft.EntityFrameworkCore;
using BultyBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BultyBook.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<CoverType> CoverType { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = default!;
        public DbSet<Company> Companies { get; set; } = default!;
        public DbSet<ShoppingCart> ShoppingCarts { get; set; } = default!;      


    }
}
