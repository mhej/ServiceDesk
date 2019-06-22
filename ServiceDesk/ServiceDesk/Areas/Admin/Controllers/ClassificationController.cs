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
    /// <summary>Provides actions for managing <see cref="Classification"/> objects.</summary>
    /// <remarks>Available for <see cref="Utilities.ApplicationUserRoles.SuperAdmin"/> role only.</remarks>
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class ClassificationController : Controller
    {
        private readonly ApplicationDbContext _db;

        /// <summary>Amount of list items per page.</summary>
        public int PageSize = 20;

        /// <summary>Initializes a new instance of the <see cref="ClassificationController"/> class.</summary>
        /// <param name="db">The database.</param>
        public ClassificationController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Admin/Classification
        /// <summary>Creates a classfications list including name as a search criteria.</summary>
        /// <param name="searchName">Name of searched <see cref="Classification"/> instances.</param>
        /// <param name="page">Page number for pagination.</param>
        /// <returns>Returns a view showing the classfications list.</returns>
        public IActionResult Index(string searchName, int page = 1)
        {
            var classifications = _db.Classifications.ToList();

            if (searchName != null)
            {
                classifications = classifications.Where(c => c.Name.ToLower().Contains(searchName.ToLower())).Select(c => c).ToList();
            }

            StringBuilder param = new StringBuilder();

            param.Append($"/Admin/Classification/Index?page=:");
            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }

            
            ClassificationsListViewModel classificationsListVM = new ClassificationsListViewModel()
            {
                Classifications = classifications.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
                SearchName = searchName,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = classifications.Count(),
                    urlParam = param.ToString()
                }
            };

            return View(classificationsListVM); 
        }

        // GET: Admin/Classification/Create
        /// <summary>Returns view for creation of <see cref="Classification"/> instance.</summary>
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Classification/Create
        /// <summary>Creates <see cref="Classification"/> object and add it to database.</summary>
        /// <param name="classification">Classification object provided by view.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Classification classification)
        {

            if (classification.Name == null)
            {
                ModelState.AddModelError("", "Classification name is required!");
            }


            if (ModelState.IsValid)
            {
                _db.Add(classification);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(classification);
        }

        // GET: Admin/Classification/Edit/5
        /// <summary>Returns view for editing of specified <see cref="Classification"/> instance.</summary>
        /// <param name="id">Specified <see cref="Classification"/> instance id.</param>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classification = await _db.Classifications.FindAsync(id);
            if (classification == null)
            {
                return NotFound();
            }
            return View(classification);
        }

        // POST: Admin/Classification/Edit/5
        /// <summary>Updates <see cref="Classification"/> object and save it to database.</summary>
        /// <param name="id">Specified <see cref="Classification"/> instance id.</param>
        /// <param name="classification">Classification object provided by view.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Classification classification)
        {
            if (id != classification.Id)
            {
                return NotFound();
            }

            if (classification.Name == null)
            {
                ModelState.AddModelError("", "Classification name is required!");
            }



            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(classification);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassificationExists(classification.Id))
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
            return View(classification);
        }

        // GET: Admin/Classification/Delete/5
        /// <summary>Returns view for deleting of specified <see cref="Classification"/> instance.</summary>
        /// <param name="id">Specified <see cref="Classification"/> instance id.</param>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classification = await _db.Classifications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classification == null)
            {
                return NotFound();
            }

            return View(classification);
        }

        // POST: Admin/Classification/Delete/5
        /// <summary>Deletes the specified <see cref="Classification"/> instance from database.</summary>
        /// <param name="id">Specified <see cref="Classification"/> instance id.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var classification = await _db.Classifications.FindAsync(id);

            List<Request> requests = _db.Requests.Where(r => r.ClassificationId == classification.Id).ToList();

            Team defaultTeam = _db.Teams.FirstOrDefault();
            ClassificationAssignedToTeam defaultClassification = new ClassificationAssignedToTeam();

            if (defaultTeam == null)
            {
                ModelState.AddModelError("", "There must be at least one team!");
                return View(classification);

            }
            else
            {
                defaultClassification = _db.ClassificationAssignedToTeam.Where(c => c.TeamId == defaultTeam.Id && c.ClassificationId!=classification.Id).FirstOrDefault();

                if (defaultClassification == null)
                {
                    ModelState.AddModelError("", $"There must be at least one category assigned to the {defaultTeam.Name} team!");
                }           
            }


            if (ModelState.IsValid)
            {
                foreach (var item in requests)
                {
                    item.Team = defaultTeam;
                    item.TeamId = defaultTeam.Id;
                    item.Classification = defaultClassification.Classification;
                    item.ClassificationId = defaultClassification.ClassificationId;
                    item.Assignee = null;
                    item.AssigneeId = null;
                }

                _db.Classifications.Remove(classification);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            return View(classification);
        }

        private bool ClassificationExists(int id)
        {
            return _db.Classifications.Any(e => e.Id == id);
        }
    }
}
