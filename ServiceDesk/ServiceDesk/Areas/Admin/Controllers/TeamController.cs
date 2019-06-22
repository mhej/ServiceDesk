using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceDesk.Data;
using ServiceDesk.Models;
using ServiceDesk.Models.ViewModels;

namespace ServiceDesk.Areas.Admin.Controllers
{
    /// <summary>Provides actions for managing <see cref="Team"/> objects.</summary>
    /// <remarks>Available for <see cref="Utilities.ApplicationUserRoles.SuperAdmin"/> role only.</remarks>
    [Area("Admin")]
    [Authorize(Roles ="SuperAdmin")]
    public class TeamController : Controller
    {
        private readonly ApplicationDbContext _db;

        /// <summary>Amount of list items per page.</summary>
        public int PageSize = 20;

        /// <summary>Initializes a new instance of the <see cref="TeamController"/> class.</summary>
        /// <param name="db">The database.</param>
        public TeamController(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <summary>Creates a teams list including name as a search criteria.</summary>
        /// <param name="searchName">Name of searched <see cref="Team"/> instances.</param>
        /// <param name="page">Page number for pagination.</param>
        // GET: Admin/Team
        public IActionResult Index(string searchName, int page = 1)
        {

            var teams = _db.Teams.ToList();

            if (searchName != null)
            {
                teams = teams.Where(t => t.Name.ToLower().Contains(searchName.ToLower())).Select(t => t).ToList();
            }

            StringBuilder param = new StringBuilder();

            param.Append($"/Admin/Team/Index?page=:");
            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }

            TeamsListViewModel teamsListVM = new TeamsListViewModel()
            {
                Teams = teams.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
                SearchName = searchName,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = teams.Count(),
                    urlParam = param.ToString()
                }
            };

            return View(teamsListVM);

        }

        // GET: Admin/Team/Create
        /// <summary>Returns view for creation of <see cref="Team"/> instance.</summary>
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Team/Create
        /// <summary>Creates <see cref="Team"/> object and add it to database.</summary>
        /// <param name="team">Team object provided by view.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Team team)
        {

            if (team.Name == null)
            {
                ModelState.AddModelError("", "Team name is required!");
            }


            if (ModelState.IsValid)
            {
                _db.Add(team);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }




        // GET: Admin/Team/Edit/5
        /// <summary>Returns view for editing of specified <see cref="Team"/> instance.</summary>
        /// <param name="id">Specified <see cref="Team"/> instance id.</param>
        public async Task<IActionResult> Edit(int? id)
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
            return View(team);
        }

        // POST: Admin/Team/Edit/5
        /// <summary>Updates <see cref="Team"/> object and save it to database.</summary>
        /// <param name="id">Specified <see cref="Team"/> instance id.</param>
        /// <param name="team">Team object provided by view.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Team team)
        {
            if (id != team.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(team);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }


        // GET: Admin/Team/Delete/5
        /// <summary>Returns view for deleting of specified <see cref="Team"/> instance.</summary>
        /// <param name="id">Specified <see cref="Team"/> instance id.</param>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _db.Teams
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Admin/Team/Delete/5
        /// <summary>Deletes the specified <see cref="Team"/> instance from database.</summary>
        /// <param name="id">Specified <see cref="Team"/> instance id.</param>
        /// <remarks>Requests which were assigned to the deleted team are provided with default team and classification.</remarks>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Team team = await _db.Teams.FindAsync(id);

            Team defaultTeam = _db.Teams.FirstOrDefault();
            ClassificationAssignedToTeam defaultClassification = _db.ClassificationAssignedToTeam.Where(c=>c.TeamId== defaultTeam.Id).FirstOrDefault();

            List<Request> requests = _db.Requests.Where(r => r.TeamId == team.Id).ToList();

            foreach (var item in requests)
            {
                item.Team = defaultTeam;
                item.TeamId = defaultTeam.Id;

                item.ClassificationId = defaultClassification.ClassificationId;
                item.Classification = _db.Classifications.Find(item.ClassificationId);

                item.Assignee = null;
                item.AssigneeId = null;

            }


            _db.Teams.Remove(team);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(int id)
        {
            return _db.Teams.Any(e => e.Id == id);
        }
    }
}
