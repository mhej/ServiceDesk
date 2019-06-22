using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Data;
using ServiceDesk.Models;
using ServiceDesk.Models.ViewModels;


namespace ServiceDesk.Areas.Admin.Controllers
{


    /// <summary>Provides actions for managing <see cref="ApplicationUser"/> objects.</summary>
    /// <remarks>Available for <see cref="Utilities.ApplicationUserRoles.SuperAdmin"/> role only.</remarks>
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class AdminUsersController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>Amount of list items per page.</summary>
        public int PageSize = 20;

        /// <summary>Initializes a new instance of the <see cref="AdminUsersController"/> class.</summary>
        /// <param name="db">The database used for dependency injection.</param>
        /// <param name="roleManager"><see cref="RoleManager{TRole}"/> instance used for dependency injection.</param>
        /// <param name="userManager"><see cref="UserManager{TUser}"/> instance used for dependency injection.</param>
        public AdminUsersController(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: AdminUsers
        /// <summary>Creates a users list including name and role as a search criteria.</summary>
        /// <param name="searchName">Name of searched <see cref="ApplicationUser"/>.</param>
        /// <param name="searchRole">Role of searched <see cref="ApplicationUser"/>.</param>
        /// <param name="page">Page number for pagination.</param>
        /// <returns>Returns a view showing the users list.</returns>
        public IActionResult Index(string searchName, string searchRole, int page = 1)
        {
            var users = _db.ApplicationUsers.ToList();

            if (searchName!=null)
            {
                users = users.Where(u => u.Name.ToLower().Contains(searchName.ToLower())).Select(u=>u).ToList();
            }

            StringBuilder param = new StringBuilder();

            param.Append($"/Admin/AdminUsers/Index?page=:");
            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }
            param.Append("&searchRole=");
            if (searchRole != null)
            {
                param.Append(searchRole);
            }

            UsersListViewModel usersListVM = new UsersListViewModel();

            usersListVM.Roles = new Dictionary<string, string>();

            List<ApplicationUser> usersWithRole = new List<ApplicationUser>();

            foreach (var user in users)
            {
                string userRoleId = _db.UserRoles.Where(u => u.UserId == user.Id).Select(r => r.RoleId).FirstOrDefault();

                string userRoleName = _db.Roles.Where(r => r.Id == userRoleId).Select(n => n.Name).FirstOrDefault();

                usersListVM.Roles.Add(user.Id, userRoleName);
            }

            if (searchRole != null)
            {

                foreach (var user in users)
                {


                    if (usersListVM.Roles[user.Id]== searchRole)
                    {
                        usersWithRole.Add(user);
                    }
                }

                users = usersWithRole;
            }

            usersListVM.Users = users.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            usersListVM.SearchName = searchName;
            usersListVM.SearchRole = searchRole;
            usersListVM.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = PageSize,
                TotalItems = users.Count(),
                urlParam = param.ToString()
            };

            return View(usersListVM);

        }

        // GET: AdminUsers/Create
        /// <summary>Returns view for <see cref="ApplicationUser"/> creation.</summary>
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminUsers/Create
        /// <summary>Creates <see cref="ApplicationUser"/> object and add it to database.</summary>
        /// <param name="adminUsersVM">AdminUsers View Model.</param>
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST(AdminUsersViewModel adminUsersVM)
        {

            ApplicationUser userCheck = _db.ApplicationUsers.Where(u => u.Email == adminUsersVM.ApplicationUser.Email).FirstOrDefault();

            if (userCheck != null)
            {
                ModelState.AddModelError("", "This user already exists!");
            }


            if (!ModelState.IsValid)
            {
                return View(adminUsersVM);
            }


            var user = new ApplicationUser
            {
                UserName = adminUsersVM.ApplicationUser.Email,
                Email = adminUsersVM.ApplicationUser.Email,
                Name = adminUsersVM.ApplicationUser.Name,
                PhoneNumber = adminUsersVM.ApplicationUser.PhoneNumber,
                EmailConfirmed = true
            };


            var result = await _userManager.CreateAsync(user, "Pass123*");

            if (result.Succeeded)
            {
                string roleName = adminUsersVM.ApplicationUserRole;

                await _userManager.AddToRoleAsync(user, roleName);
            }

            _db.ApplicationUsers.Add(user);

            return RedirectToAction(nameof(Index));

        }




        // GET: AdminUsers/Edit/5

        /// <summary>Returns view for specified <see cref="ApplicationUser"/> editing.</summary>
        /// <param name="id"><see cref="ApplicationUser"/> id.</param>
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();

            }

            var userFromDb = await _db.ApplicationUsers.FindAsync(id);
            if (userFromDb == null)
            {
                return RedirectToAction(nameof(Index));
            }

            AdminUsersViewModel adminUsersVM = new AdminUsersViewModel();

            adminUsersVM.ApplicationUser = userFromDb;

            string userRoleId = _db.UserRoles.Where(u=>u.UserId==id).Select(r => r.RoleId).FirstOrDefault();

            string userRoleName = _db.Roles.Where(r => r.Id == userRoleId).Select(n => n.Name).FirstOrDefault();

            adminUsersVM.ApplicationUserRole = userRoleName;

            return View(adminUsersVM);
        }

        // POST: AdminUsers/Edit/5
        /// <summary>Update <see cref="ApplicationUser"/> object and save it to database.</summary>
        /// <param name="adminUsersVM">AdminUsers View Model.</param>

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminUsersViewModel adminUsersVM)
        {


            if (ModelState.IsValid)
            {
                ApplicationUser userFromDb = _db.ApplicationUsers.Where(u => u.Id == adminUsersVM.ApplicationUser.Id).FirstOrDefault();
                userFromDb.Name = adminUsersVM.ApplicationUser.Name;
                userFromDb.PhoneNumber = adminUsersVM.ApplicationUser.PhoneNumber;

             
                string userRoleId = _db.UserRoles.Where(u => u.UserId == adminUsersVM.ApplicationUser.Id).Select(r => r.RoleId).FirstOrDefault();

                string userRoleName = _db.Roles.Where(r => r.Id == userRoleId).Select(n => n.Name).FirstOrDefault();

                if (userRoleName != null)
                {
                    IdentityResult deletionResult = await _userManager.RemoveFromRoleAsync(userFromDb, userRoleName);
                }
                
                string newRole = adminUsersVM.ApplicationUserRole;

                await _userManager.AddToRoleAsync(userFromDb, newRole);

                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
          
            return View(adminUsersVM);
        }



        // GET: AdminUsers/Delete/5
        /// <summary>Returns view for specified <see cref="ApplicationUser"/> inactivation.</summary>
        /// <param name="id"><see cref="ApplicationUser"/> id.</param>
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }

            var userFromDb = await _db.ApplicationUsers.FindAsync(id);
            if (userFromDb == null)
            {
                return NotFound();
            }

            return View(userFromDb);
        }

        // POST: AdminUsers/Delete/5
        /// <summary>Inactivates the specified <see cref="ApplicationUser"/>.</summary>
        /// <param name="id"><see cref="ApplicationUser"/> id.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(string id)
        {
            ApplicationUser userFromDb = _db.ApplicationUsers.Where(u => u.Id == id).FirstOrDefault();

            foreach (var adminAssigned in _db.AdminAssignedToTeam)
            {
                if (adminAssigned.ApplicationUserId==userFromDb.Id)
                {
                    _db.AdminAssignedToTeam.Remove(adminAssigned);
                }
            }

            userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);

            

            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        // GET
        /// <summary>Returns view for specified <see cref="ApplicationUser"/> activation.</summary>
        /// <param name="id"><see cref="ApplicationUser"/> id.</param>
        public async Task<IActionResult> Activate(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }

            var userFromDb = await _db.ApplicationUsers.FindAsync(id);
            if (userFromDb == null)
            {
                return NotFound();
            }

            return View(userFromDb);
        }


        // POST: 
        /// <summary>Activates the specified <see cref="ApplicationUser"/>.</summary>
        /// <param name="id"><see cref="ApplicationUser"/> id.</param>
        [HttpPost, ActionName("Activate")]
        [ValidateAntiForgeryToken]
        public IActionResult ActivatePOST(string id)
        {
            ApplicationUser userFromDb = _db.ApplicationUsers.Where(u => u.Id == id).FirstOrDefault();



            userFromDb.LockoutEnd = null;



            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



    }
}