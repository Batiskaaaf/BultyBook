using BultyBook.DataAccess.Data;
using BultyBook.Models;
using BultyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BultyBook.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        //apply migrations if they are not applied

        //if roles are not created, create admin user

        //create roles if there are no roles

        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext DbContext;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext applicationDbContext
            )
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.DbContext = applicationDbContext;
        }

        public void Initialize()
        {
            try
            {
                if(DbContext.Database.GetPendingMigrations().Count() > 0)
                {
                    DbContext.Database.Migrate();
                }
            }
            catch(Exception ex)
            {

            }

            if (!roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.Role_User_Indi)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(SD.Role_User_Comp)).GetAwaiter().GetResult();

                userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@bultybook.com",
                    Email = "admin@bultybook.com",
                    Name = "Admin Admin",
                    PhoneNumber = "9999999999",
                    StreetAddress = "ADMIN STREEET",
                    State = "ADMIN STATE",
                    PostalCode = "99999",
                    City = "ADMIN CITY",
                },"Admin1!").GetAwaiter().GetResult();

                ApplicationUser user = DbContext.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@bultybook.com");

                userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }
            return;
        }
    }
}
