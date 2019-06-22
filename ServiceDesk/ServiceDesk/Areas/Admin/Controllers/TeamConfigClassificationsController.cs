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
    /// <summary>Provides actions for managing <see cref="Classification"/> objects assigned to the specified <see cref="Team"/>.</summary>
    /// <remarks>Available for <see cref="Utilities.ApplicationUserRoles.SuperAdmin"/> role only.</remarks>
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class TeamConfigClassificationsController : Controller
    {
        private readonly ApplicationDbContext _db;


        /// <summary>Initializes a new instance of the <see cref="TeamConfigClassificationsController"/> class.</summary>
        /// <param name="db">The database.</param>
        public TeamConfigClassificationsController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET
        /// <summary>Provides a list of <see cref="Classification"/> objects assigned to the specified <see cref="Team"/></summary>
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

            TeamConfigClassificationsViewModel teamConfigClassificationsVM = new TeamConfigClassificationsViewModel();

            teamConfigClassificationsVM.Team = team;
            teamConfigClassificationsVM.AllClassifications = new List<Classification>();

            foreach (var classification in _db.Classifications)
            {
                    teamConfigClassificationsVM.AllClassifications.Add(classification);
            }

            teamConfigClassificationsVM.ClassificationsAssignedToTeam = new List<Classification>();
            foreach (var classAssigned in _db.ClassificationAssignedToTeam)
            {
                foreach (var classification in teamConfigClassificationsVM.AllClassifications)
                {
                    if (classAssigned.ClassificationId == classification.Id && classAssigned.TeamId == teamConfigClassificationsVM.Team.Id)
                    {
                        teamConfigClassificationsVM.ClassificationsAssignedToTeam.Add(classAssigned.Classification);
                    }
                }
            }

            return View(teamConfigClassificationsVM);
        }

        /// <summary>Create <see cref="ClassificationAssignedToTeam"/> object and save it in database.</summary>
        /// <param name="id">The identifier of the specified <see cref="Classification"/>.</param>
        /// <param name="teamConfigClassificationsVM">Instance of <see cref="TeamConfigClassificationsViewModel"/> provided by View.</param>
        [HttpPost]
        public async Task<IActionResult> Add(int id, TeamConfigClassificationsViewModel teamConfigClassificationsVM)
        {
            if (ModelState.IsValid)
            {
                ClassificationAssignedToTeam classificationAssignedToTeam = new ClassificationAssignedToTeam()
                {
                    ClassificationId = id,
                    TeamId = teamConfigClassificationsVM.Team.Id,
                    Classification = await _db.Classifications.FindAsync(id),
                    Team = await _db.Teams.FindAsync(teamConfigClassificationsVM.Team.Id)

                };


                foreach (var classAssigned in _db.ClassificationAssignedToTeam)
                {
                    if (classAssigned.ClassificationId == classificationAssignedToTeam.ClassificationId && classAssigned.TeamId == classificationAssignedToTeam.TeamId)
                    {
                        return RedirectToAction("Index", new { id = teamConfigClassificationsVM.Team.Id });
                    }
                }


                await _db.ClassificationAssignedToTeam.AddAsync(classificationAssignedToTeam);


                TempData["Msg"] = "Classification has been added successfully";

                await _db.SaveChangesAsync();

                return RedirectToAction("Index", new { id = teamConfigClassificationsVM.Team.Id });

            }

            return RedirectToAction("Index","Team");

        }




        /// <summary>Delete the specified <see cref="ClassificationAssignedToTeam"/> object from database.</summary>
        /// <param name="id">The identifier of the specified <see cref="Classification"/>.</param>
        /// <param name="teamConfigClassificationsVM">Instance of <see cref="TeamConfigClassificationsViewModel"/> provided by View.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, TeamConfigClassificationsViewModel teamConfigClassificationsVM)
        {
            ClassificationAssignedToTeam classificationAssignedToTeam = new ClassificationAssignedToTeam()
            {
                ClassificationId = id,
                TeamId = teamConfigClassificationsVM.Team.Id,
                Classification = await _db.Classifications.FindAsync(id),
                Team = await _db.Teams.FindAsync(teamConfigClassificationsVM.Team.Id)

            };

            foreach (var classAssigned in _db.ClassificationAssignedToTeam)
            {
                if (classAssigned.ClassificationId == classificationAssignedToTeam.ClassificationId && classAssigned.TeamId == classificationAssignedToTeam.TeamId)
                {
                    _db.ClassificationAssignedToTeam.Remove(classAssigned);
                    TempData["Msg"] = "Classification has been removed successfully";
                }
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index", new { id = teamConfigClassificationsVM.Team.Id });
        }
    }
}