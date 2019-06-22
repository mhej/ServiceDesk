using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Data;
using ServiceDesk.Models;
using ServiceDesk.Models.ViewModels;

namespace ServiceDesk.Areas.Admin.Controllers
{



    /// <summary>Provides actions for managing <see cref="ApplicationUser"/> objects assigned to the specified <see cref="Team"/>.</summary>
    /// <remarks>Available for <see cref="Utilities.ApplicationUserRoles.SuperAdmin"/> role only.</remarks>
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class TeamConfigUsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>Initializes a new instance of the <see cref="TeamConfigUsersController"/> class.</summary>
        /// <param name="db">The database used for dependency injection.</param>
        /// <param name="userManager"><see cref="UserManager{TUser}"/> instance used for dependency injection.</param>
        public TeamConfigUsersController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        //GET
        /// <summary>Provides a list of <see cref="ApplicationUser"/> objects assigned to the specified <see cref="Team"/></summary>
        /// <param name="id">The specified <see cref="Team"/> id.</param>
        public async Task<IActionResult> Index(int? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

        
            var team = await _db.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            TeamConfigUsersViewModel teamConfigUsersVM = new TeamConfigUsersViewModel();

            teamConfigUsersVM.Team = team;
            teamConfigUsersVM.AllAdmins = new List<ApplicationUser>();

            foreach (var user in _db.ApplicationUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin")|| await _userManager.IsInRoleAsync(user, "SuperAdmin"))
                {
                    teamConfigUsersVM.AllAdmins.Add(user);
                }
            }

            teamConfigUsersVM.AdminsAssignedToTeam = new List<ApplicationUser>();
            foreach (var adminAssigned in _db.AdminAssignedToTeam)
            {
                foreach (var admin in teamConfigUsersVM.AllAdmins)
                {
                    if (adminAssigned.ApplicationUserId == admin.Id && adminAssigned.TeamId == teamConfigUsersVM.Team.Id)
                    {
                        teamConfigUsersVM.AdminsAssignedToTeam.Add(admin);
                    }
                }
            }





            return View(teamConfigUsersVM);
        }

        /// <summary>Create <see cref="AdminAssignedToTeam"/> object and save it in database.</summary>
        /// <param name="id">The identifier of the specified <see cref="ApplicationUser"/>.</param>
        /// <param name="teamConfigUsersVM">Instance of <see cref="TeamConfigUsersViewModel"/> provided by View.</param>
        [HttpPost]
        public async Task<IActionResult> Add(string id, TeamConfigUsersViewModel teamConfigUsersVM)
        {
            if (ModelState.IsValid)
            {
                AdminAssignedToTeam adminAssignedToTeam = new AdminAssignedToTeam()
                {
                    ApplicationUserId = id,
                    TeamId = teamConfigUsersVM.Team.Id,
                    ApplicationUser = await _db.ApplicationUsers.FindAsync(id),
                    Team = await _db.Teams.FindAsync(teamConfigUsersVM.Team.Id)

                };


                foreach (var adminAssigned in _db.AdminAssignedToTeam)
                {
                    if (adminAssigned.ApplicationUserId == adminAssignedToTeam.ApplicationUserId && adminAssigned.TeamId == adminAssignedToTeam.TeamId)
                    {
                        return RedirectToAction("Index", new { id = teamConfigUsersVM.Team.Id });
                    }
                }


                await _db.AdminAssignedToTeam.AddAsync(adminAssignedToTeam);


                TempData["Msg"] = "Admin has been added successfully";

                await _db.SaveChangesAsync();

                return RedirectToAction("Index", new { id = teamConfigUsersVM.Team.Id });

            }

            return RedirectToAction("Index", "Team");

        }


        /// <summary>Delete the specified <see cref="AdminAssignedToTeam"/> object from database.</summary>
        /// <param name="id">The identifier of the specified <see cref="ApplicationUser"/>.</param>
        /// <param name="teamConfigUsersVM">Instance of <see cref="TeamConfigUsersViewModel"/> provided by View.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id, TeamConfigUsersViewModel teamConfigUsersVM)
        {
            AdminAssignedToTeam adminAssignedToTeam = new AdminAssignedToTeam()
            {
                ApplicationUserId = id,
                TeamId = teamConfigUsersVM.Team.Id,
                ApplicationUser = await _db.ApplicationUsers.FindAsync(id),
                Team = await _db.Teams.FindAsync(teamConfigUsersVM.Team.Id)

            };

            foreach (var adminAssigned in _db.AdminAssignedToTeam)
            {
                if (adminAssigned.ApplicationUserId == adminAssignedToTeam.ApplicationUserId && adminAssigned.TeamId == adminAssignedToTeam.TeamId)
                {
                    _db.AdminAssignedToTeam.Remove(adminAssigned);
                    TempData["Msg"] = "Admin has been removed successfully";
                }
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index", new { id = teamConfigUsersVM.Team.Id });
        }





    }





}