using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Data;
using ServiceDesk.Models;
using ServiceDesk.Models.ViewModels;

namespace ServiceDesk.Areas.RequestConfig.Controllers
{
    /// <summary>Provides actions for managing <see cref="RequestNote"/> objects.</summary>
    [Area("RequestConfig")]
    [Authorize]
    public class RequestNotesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>Initializes a new instance of the <see cref="RequestNotesController"/> class.</summary>
        /// <param name="db">The database used for dependency injection.</param>
        /// <param name="userManager"><see cref="UserManager{TUser}"/> instance used for dependency injection.</param>
        public RequestNotesController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // POST: RequestNotes/Create
        /// <summary>Creates <see cref="RequestNote"/> object, passes it to <see cref="NotesAssignedToRequest"/> object and save it in database.</summary>
        /// <param name="requestVM">Instance of <see cref="RequestViewModel"/> provided by View.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequestViewModel requestVM)
        {
            if (requestVM.RequestNote.Content ==null)
            {
                return RedirectToAction("Edit", "Requests", new { id = requestVM.Request.Id });
            }

            if (ModelState.IsValid)
            {

                requestVM.RequestNote.Date = DateTime.Now;
                requestVM.RequestNote.NoteUser = (ApplicationUser) await _userManager.GetUserAsync(HttpContext.User);

                await _db.RequestNotes.AddAsync(requestVM.RequestNote);

                NotesAssignedToRequest noteAssignedToRequest = new NotesAssignedToRequest()
                {
                    
                    RequestId = requestVM.Request.Id,
                    Request = await _db.Requests.FindAsync(requestVM.Request.Id),
                    RequestNote = requestVM.RequestNote,

                };

                await _db.NotesAssignedToRequest.AddAsync(noteAssignedToRequest);
           
                await _db.SaveChangesAsync();

                return RedirectToAction("Edit", "Requests", new { id = requestVM.Request.Id });
            }

            return RedirectToAction("Edit", "Requests", new { id = requestVM.Request.Id });
            
        }
    }
}