using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Research_Repository.Data;
using Research_Repository_Models;
using Research_Repository_Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Research_Repository_DataAccess.Initialiser
{
    public class DbInitialiser : IDbInitialiser
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitialiser(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initialise()
        {
            try
            {
                if(_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch(Exception ex)
            {

            }

            //If foles do not exist in database, create them
            if (!_roleManager.RoleExistsAsync(WC.LibrarianRole).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(WC.LibrarianRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(WC.UploaderRole)).GetAwaiter().GetResult();
            } else
            {
                return;
            }

            //Create first user
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "librarian@research.com",
                Email = "librarian@research.com",
                FirstName = "John",
                LastName = "Doe",
                TeamId = null
            }, "Password123$").GetAwaiter().GetResult();

            //Assign new user to librarian role
            ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "librarian@research.com");
            _userManager.AddToRoleAsync(user, WC.LibrarianRole).GetAwaiter().GetResult();
        }
    }
}
