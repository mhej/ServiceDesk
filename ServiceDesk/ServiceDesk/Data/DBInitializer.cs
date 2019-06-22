using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceDesk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Data
{
    /// <summary>Provides methods to be used during database initialization.</summary>
    public class DBInitializer: IDBInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>Initializes a new instance of the <see cref="DBInitializer"/> class.</summary>
        /// <param name="db">The database used for dependency injection.</param>
        /// <param name="roleManager"><see cref="RoleManager{TRole}"/> instance used for dependency injection.</param>
        /// <param name="userManager"><see cref="UserManager{TUser}"/> instance used for dependency injection.</param>
        public DBInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        /// <summary>Creates user roles: Requestor, Admin, SuperAdmin. Also, creates a user with SuperAdmin role using default name and password. </summary>
        public async void Initialize()
        {
            if (_db.Database.GetPendingMigrations().Count() > 0)
            {
                _db.Database.Migrate();
            }


            if (_db.Roles.Any(r => r.Name == "superadmin")) return;

            _roleManager.CreateAsync(new IdentityRole("Requestor")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("SuperAdmin")).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "superadmin",
                Name = "superadmin",
                EmailConfirmed = true
            }, "Admin123*").GetAwaiter().GetResult();

            IdentityUser user = await _db.Users.Where(u => u.UserName == "superadmin").FirstOrDefaultAsync();

            await _userManager.AddToRoleAsync(user, "SuperAdmin");

            
        }

    }
}
