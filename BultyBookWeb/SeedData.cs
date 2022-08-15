using Microsoft.EntityFrameworkCore;
using BultyBook.DataAccess.Data;
using BultyBook.Models;


namespace BultyBookWeb
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ApplicationDbContext>>()))
            {
                // Look for any movies.
                if (context.Category.Any())
                {
                    return;   // DB has been seeded
                }

                context.Category.AddRange(
                    new Category
                    {
                        Name = "Comedy",
                        DisplayOrder = 1
                    },

                    new Category
                    {
                        Name = "Horor",
                        DisplayOrder = 2
                    },

                    new Category
                    {
                        Name = "Fantasy",
                        DisplayOrder = 3
                    }
                );
                context.SaveChanges();
            }
        }
    }
}