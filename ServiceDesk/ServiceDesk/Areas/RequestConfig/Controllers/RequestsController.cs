using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ServiceDesk.Data;
using ServiceDesk.Models;
using ServiceDesk.Models.ViewModels;
using ServiceDesk.Utilities;


namespace ServiceDesk.Areas.RequestConfig.Controllers
{
    /// <summary>Provides actions for managing <see cref="Request"/> objects.</summary>
    [Area("RequestConfig")]
    [Authorize]
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly HostingEnvironment _hostingEnvironment;

        /// <summary>Amount of list items per page.</summary>
        public int PageSize = 20;

        /// <summary>Initializes a new instance of the <see cref="RequestsController"/> class.</summary>
        /// <param name="db">The database used for dependency injection.</param>
        /// <param name="userManager"><see cref="UserManager{TUser}"/> instance used for dependency injection.</param>
        /// <param name="hostingEnvironment">The <see cref="HostingEnvironment"/> instance used for dependency injection.</param>
        public RequestsController(ApplicationDbContext db, UserManager<IdentityUser> userManager, HostingEnvironment hostingEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;

        }

        // GET: Request/Requests

        /// <summary>Creates a requests list based on specified search criteria.</summary>
        /// <param name="searchTitle">Title of the searched request.</param>
        /// <param name="searchClassification">Category of the searched request.</param>
        /// <param name="searchTeam">Team assigned to the searched request.</param>
        /// <param name="searchRequestor">Requestor of the searched request.</param>
        /// <param name="searchAssignee">Admin assigned to searched request.</param>
        /// <param name="searchStatus">Status of the searched request.</param>
        /// <param name="searchSubmitDateFrom">The start of submit date range of the searched request.</param>
        /// <param name="searchSubmitDateTo">The end of submit date range of the searched request.</param>
        /// <param name="page">Page number for pagination.</param>
        /// <remarks>If user's role is <see cref="Utilities.ApplicationUserRoles.Requestor"/>, <see cref="IndexRequestsSubmittedByUser"/> is called.</remarks>
        public async Task<IActionResult> Index(string searchTitle, string searchClassification, string searchTeam, string searchRequestor, string searchAssignee, string searchStatus, string searchSubmitDateFrom, string searchSubmitDateTo, int page = 1)
        {

            var currentUserRoleName = await GetCurrentUserRoleNameAsync();

            if (currentUserRoleName == "Requestor")
            {
                return RedirectToAction("IndexRequestsSubmittedByUser");
            }

            var currentUser = await GetCurrentUserAsync();
            var requests = GetFilteredRequests(searchTitle, searchClassification, searchTeam, searchRequestor, searchAssignee, searchStatus, searchSubmitDateFrom, searchSubmitDateTo).ToList();

            List<Request> requestsByTeam = new List<Request>();

            string urlIndex = BuildIndexUrl("Index", searchTitle, searchClassification, searchTeam, searchRequestor, searchAssignee, searchStatus, searchSubmitDateFrom, searchSubmitDateTo);

            if (currentUserRoleName == "Admin")
            {
                var currentUserTeamId = _db.AdminAssignedToTeam.Where(a => a.ApplicationUserId == currentUser.Id).Select(a => a.Team.Id).ToList();

                foreach (var i in currentUserTeamId)
                {
                    var result = requests.Where(r => r.Team.Id == i).Select(r => r).ToList();

                    requestsByTeam.AddRange(result);
                }

                requests = requestsByTeam;

            }


            RequestsListViewModel requestsListVM = new RequestsListViewModel()
            {

                Requests = requests.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
                SearchTitle = searchTitle,
                SearchClassification = searchClassification,
                SearchTeam = searchTeam,
                SearchRequestor = searchRequestor,
                SearchAssignee = searchAssignee,
                SearchStatus = searchStatus,
                SearchSubmitDateFrom = searchSubmitDateFrom,
                SearchSubmitDateTo = searchSubmitDateTo,


                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = requests.Count(),
                    urlParam = urlIndex

                }

            };

            return View(requestsListVM);


        }

        // GET: Request/Requests/Details/5
        /// <summary>Shows the specified <see cref="Request"/> object details.</summary>
        /// <param name="id">Identifier of Request object.</param>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _db.Requests
                                .Include(r => r.Assignee)
                                .Include(r => r.Classification)
                                .Include(r => r.Requestor)
                                .Include(r => r.Team)
                                .FirstOrDefaultAsync(m => m.Id == id);

            RequestViewModel requestVM = new RequestViewModel()
            {
                Request = request,
                Teams = _db.Teams.ToList(),
                Classifications = _db.ClassificationAssignedToTeam.Where(c => c.TeamId == request.TeamId).Select(c => c.Classification).ToList(),
                Assignees = _db.AdminAssignedToTeam.Where(a => a.TeamId == request.TeamId).Select(a => a.ApplicationUser).ToList(),
                RequestNotes = _db.NotesAssignedToRequest.Where(n => n.RequestId == id).Select(n => n.RequestNote).ToList(),
                Images = _db.ImagesAssignedToRequest.Where(i => i.RequestId == id).Select(i => i.Image).ToList()
            };


            if (request == null)
            {
                return NotFound();
            }

            return View(requestVM);
        }

        // GET: Request/Requests/Create
        /// <summary>Returns view for creation of <see cref="Request"/> instance.</summary>
        public IActionResult Create()
        {

            return View();
        }

        // POST: Request/Requests/Create
        /// <summary>Creates <see cref="Request"/> object and add it to database.</summary>
        /// <param name="request">Request object provided by view.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Request request)
        {
            Team defaultTeam = _db.Teams.FirstOrDefault();
            ClassificationAssignedToTeam defaultClassification = new ClassificationAssignedToTeam();

            if (defaultTeam ==null)
            {
                ModelState.AddModelError("", "There must be at least one team!");
            }
            else
            {
                defaultClassification = _db.ClassificationAssignedToTeam.Where(c => c.TeamId == defaultTeam.Id).FirstOrDefault();

                if (defaultClassification == null)
                {
                    ModelState.AddModelError("", $"There must be at least one category assigned to the {defaultTeam.Name} team!");
                }

            }
                        
            if (ModelState.IsValid)
            {
                request.Status = Utilities.RequestStatus.New;
                request.SubmitDate = DateTime.Now;
                request.Team = defaultTeam;
                request.Requestor = (ApplicationUser)await _userManager.GetUserAsync(HttpContext.User);
                request.Classification = _db.Classifications.Find(defaultClassification.ClassificationId);

                _db.Add(request);
                await _db.SaveChangesAsync();



                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var requestFromDb = _db.Requests.Find(request.Id);



                if (files.Count != 0)
                {
                    //Image has been uploaded
                    var uploads = Path.Combine(webRootPath, @"images\screens\");
                    var fileName = files[0].FileName;

                    using (var filestream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);

                    }

                    Image image = new Image()
                    {
                        Name = fileName
                    };

                    _db.Image.Add(image);

                    ImagesAssignedToRequest imageAssignedToRequest = new ImagesAssignedToRequest()
                    {

                        RequestId = request.Id,
                        Request = await _db.Requests.FindAsync(request.Id),
                        Image = image
                    };

                    await _db.ImagesAssignedToRequest.AddAsync(imageAssignedToRequest);

                }

                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Requests", new { id = request.Id });



            }

            return View(request);
        }

        // GET: Request/Requests/Edit/5
        /// <summary>Returns view for editing of specified <see cref="Request"/> instance.</summary>
        /// <param name="id">Specified <see cref="Request"/> instance id.</param>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _db.Requests
                                .Include(r => r.Assignee)
                                .Include(r => r.Classification)
                                .Include(r => r.Requestor)
                                .Include(r => r.Team)
                                .FirstOrDefaultAsync(m => m.Id == id);

            RequestViewModel requestVM = new RequestViewModel()
            {
                Request = request,
                Teams = _db.Teams.ToList(),
                Classifications = _db.ClassificationAssignedToTeam.Where(c => c.TeamId == request.TeamId).Select(c => c.Classification).ToList(),
                Assignees = _db.AdminAssignedToTeam.Where(a => a.TeamId == request.TeamId).Select(a => a.ApplicationUser).ToList(),
                RequestNotes = _db.NotesAssignedToRequest.Where(n => n.RequestId == id).Select(n => n.RequestNote).ToList(),
                Images = _db.ImagesAssignedToRequest.Where(i => i.RequestId == id).Select(i => i.Image).ToList()
            };


            if (request == null)
            {
                return NotFound();
            }

            return View(requestVM);
        }

        // POST: Request/Requests/Edit/5
        /// <summary>Updates <see cref="Request"/> object and save it to database.</summary>
        /// <param name="requestVM">Instance of <see cref="RequestViewModel"/> provided by View.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(RequestViewModel requestVM)
        {
            if (ModelState.IsValid)
            {




                requestVM.Request.Requestor = _db.ApplicationUsers.Where(u => u.Id == requestVM.Request.RequestorId).FirstOrDefault();
                requestVM.Request.Classification = _db.Classifications.Where(c => c.Id == requestVM.Request.ClassificationId).FirstOrDefault();
                requestVM.Request.Assignee = _db.ApplicationUsers.Where(u => u.Id == requestVM.Request.AssigneeId).FirstOrDefault();
                requestVM.Request.Team = _db.Teams.Where(t => t.Id == requestVM.Request.TeamId).FirstOrDefault();

                if (requestVM.Request.Status == RequestStatus.Closed)
                {
                    var statusInDB = _db.Requests.Where(r => r.Id == requestVM.Request.Id).Select(r => r.Status).FirstOrDefault();
                    if (statusInDB != RequestStatus.Closed)
                    {
                        requestVM.Request.ClosedDate = DateTime.Now;
                    }
                }

                _db.Update(requestVM.Request);

                _db.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(requestVM.Request);
        }


        /// <summary>Gets the classifications assigned to the specified team.</summary>
        /// <param name="teamId">The team identifier.</param>
        /// <returns>SelectList to be used in a View.</returns>
        public JsonResult GetClassification(int teamId)
        {
            List<Classification> classifications = new List<Classification>();

            classifications = _db.ClassificationAssignedToTeam.Where(c => c.TeamId == teamId).Select(c => c.Classification).ToList();

            return Json(new SelectList(classifications, "Id", "Name"));
        }

        /// <summary>Gets the admins assigned to the specified team.</summary>
        /// <param name="teamId">The team identifier.</param>
        /// <returns>SelectList to be used in a View.</returns>
        public JsonResult GetAssignee(int teamId)
        {
            List<ApplicationUser> assignees = new List<ApplicationUser>();

            assignees = _db.AdminAssignedToTeam.Where(u => u.TeamId == teamId).Select(u => u.ApplicationUser).ToList();

            assignees.Insert(0, new ApplicationUser { Id = null, Name = null });

            return Json(new SelectList(assignees, "Id", "Name"));
        }

        /// <summary>Creates a requests list based on specified search criteria. The list shows requests assigned to the current application user.</summary>
        /// <param name="searchTitle">Title of the searched request.</param>
        /// <param name="searchClassification">Category of the searched request.</param>
        /// <param name="searchTeam">Team assigned to the searched request.</param>
        /// <param name="searchRequestor">Requestor of the searched request.</param>
        /// <param name="searchAssignee">Admin assigned to searched request.</param>
        /// <param name="searchStatus">Status of the searched request.</param>
        /// <param name="searchSubmitDateFrom">Starting date of submission of the searched request.</param>
        /// <param name="searchSubmitDateTo">Ending date of submission of the searched request.</param>
        /// <param name="page">Page number for pagination.</param>
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> IndexRequestsAssignedToUser(string searchTitle, string searchClassification, string searchTeam, string searchRequestor, string searchAssignee, string searchStatus, string searchSubmitDateFrom, string searchSubmitDateTo, int page = 1)
        {
            var requests = GetFilteredRequests(searchTitle, searchClassification, searchTeam, searchRequestor, searchAssignee, searchStatus, searchSubmitDateFrom, searchSubmitDateTo);



            ApplicationUser currentUser = await GetCurrentUserAsync();


            requests = requests.Where(r => r.Assignee.Id == currentUser.Id).Select(r => r)
                    .Include(r => r.Assignee).Include(r => r.Classification).Include(r => r.Requestor).Include(r => r.Team);


            string urlIndex = BuildIndexUrl("IndexRequestsAssignedToUser", searchTitle, searchClassification, searchTeam, searchRequestor, searchAssignee, searchStatus, searchSubmitDateFrom, searchSubmitDateTo);


            RequestsListViewModel requestsListVM = new RequestsListViewModel()
            {
                Requests = requests.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
                SearchTitle = searchTitle,
                SearchClassification = searchClassification,
                SearchTeam = searchTeam,
                SearchRequestor = searchRequestor,
                SearchAssignee = searchAssignee,
                SearchStatus = searchStatus,
                SearchSubmitDateFrom = searchSubmitDateFrom,
                SearchSubmitDateTo = searchSubmitDateTo,

                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = requests.Count(),
                    urlParam = urlIndex
                }

            };

            return View(requestsListVM);
        }

        /// <summary>Creates a requests list based on specified search criteria. The list shows requests submitted by the current application user.</summary>
        /// <param name="searchTitle">Title of the searched request.</param>
        /// <param name="searchClassification">Category of the searched request.</param>
        /// <param name="searchTeam">Team assigned to the searched request.</param>
        /// <param name="searchRequestor">Requestor of the searched request.</param>
        /// <param name="searchAssignee">Admin assigned to searched request.</param>
        /// <param name="searchStatus">Status of the searched request.</param>
        /// <param name="searchSubmitDateFrom">Starting date of submission of the searched request.</param>
        /// <param name="searchSubmitDateTo">Ending date of submission of the searched request.</param>
        /// <param name="page">Page number for pagination.</param>
        public async Task<IActionResult> IndexRequestsSubmittedByUser(string searchTitle, string searchClassification, string searchTeam, string searchRequestor, string searchAssignee, string searchStatus, string searchSubmitDateFrom, string searchSubmitDateTo, int page = 1)
        {

            var requests = GetFilteredRequests(searchTitle, searchClassification, searchTeam, searchRequestor, searchAssignee, searchStatus, searchSubmitDateFrom, searchSubmitDateTo);



            ApplicationUser currentUser = await GetCurrentUserAsync();

            requests = requests.Where(r => r.Requestor.Id == currentUser.Id).Select(r => r)
                    .Include(r => r.Assignee).Include(r => r.Classification).Include(r => r.Requestor).Include(r => r.Team);


            string urlIndex = BuildIndexUrl("IndexRequestsSubmittedByUser", searchTitle, searchClassification, searchTeam, searchRequestor, searchAssignee, searchStatus, searchSubmitDateFrom, searchSubmitDateTo);


            RequestsListViewModel requestsListVM = new RequestsListViewModel()
            {
                Requests = requests.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
                SearchTitle = searchTitle,
                SearchClassification = searchClassification,
                SearchTeam = searchTeam,
                SearchRequestor = searchRequestor,
                SearchAssignee = searchAssignee,
                SearchStatus = searchStatus,
                SearchSubmitDateFrom = searchSubmitDateFrom,
                SearchSubmitDateTo = searchSubmitDateTo,

                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = requests.Count(),
                    urlParam = urlIndex
                }

            };

            return View(requestsListVM);
        }

        /// <summary>Gets the filtered requests.</summary>
        /// <param name="searchTitle">Title of the searched request.</param>
        /// <param name="searchClassification">Category of the searched request.</param>
        /// <param name="searchTeam">Team assigned to the searched request.</param>
        /// <param name="searchRequestor">Requestor of the searched request.</param>
        /// <param name="searchAssignee">Admin assigned to searched request.</param>
        /// <param name="searchStatus">Status of the searched request.</param>
        /// <param name="searchSubmitDateFrom">Starting date of submission of the searched request.</param>
        /// <param name="searchSubmitDateTo">Ending date of submission of the searched request.</param>
        private IIncludableQueryable<Request, Team> GetFilteredRequests(string searchTitle, string searchClassification, string searchTeam, string searchRequestor, string searchAssignee, string searchStatus, string searchSubmitDateFrom, string searchSubmitDateTo)
        {
            var requests = _db.Requests.Include(r => r.Assignee).Include(r => r.Classification).Include(r => r.Requestor).Include(r => r.Team);

            if (searchTitle != null)
            {
                requests = requests.Where(r => r.Title.ToLower().Contains(searchTitle.ToLower()))
                    .Include(r => r.Assignee).Include(r => r.Classification).Include(r => r.Requestor).Include(r => r.Team);
            }
            if (searchClassification != null)
            {
                requests = requests.Where(r => r.Classification.Name.ToLower().Contains(searchClassification.ToLower()))
                    .Include(r => r.Assignee).Include(r => r.Classification).Include(r => r.Requestor).Include(r => r.Team);
            }
            if (searchTeam != null)
            {
                requests = requests.Where(r => r.Team.Name.ToLower().Contains(searchTeam.ToLower()))
                    .Include(r => r.Assignee).Include(r => r.Classification)
                    .Include(r => r.Requestor).Include(r => r.Team);
            }
            if (searchRequestor != null)
            {
                requests = requests.Where(r => r.Requestor.Name.ToLower().Contains(searchRequestor.ToLower()))
                    .Include(r => r.Assignee).Include(r => r.Classification).Include(r => r.Requestor).Include(r => r.Team);
            }
            if (searchAssignee != null)
            {
                requests = requests.Where(r => r.Assignee.Name.ToLower().Contains(searchAssignee.ToLower()))
                    .Include(r => r.Assignee).Include(r => r.Classification).Include(r => r.Requestor).Include(r => r.Team);
            }
            if (searchStatus != null)
            {
                requests = requests.Where(r => r.Status.ToString() == searchStatus)
                    .Include(r => r.Assignee).Include(r => r.Classification).Include(r => r.Requestor).Include(r => r.Team);
            }
            if (searchSubmitDateFrom != null)
            {


                DateTime submitDate = Convert.ToDateTime(searchSubmitDateFrom);

                requests = (from r in requests
                            where r.SubmitDate >= submitDate
                            select r)
                           .Include(r => r.Assignee).Include(r => r.Classification).Include(r => r.Requestor).Include(r => r.Team);

            }
            if (searchSubmitDateTo != null)
            {


                DateTime submitDate = Convert.ToDateTime(searchSubmitDateTo);

                requests = (from r in requests
                            where r.SubmitDate <= submitDate.AddHours(23).AddMinutes(59).AddSeconds(59)
                            select r)
                           .Include(r => r.Assignee).Include(r => r.Classification).Include(r => r.Requestor).Include(r => r.Team);

            }



            return requests;

        }

        /// <summary>Gets the current user.</summary>
        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            ApplicationUser currentUser = (ApplicationUser)await _userManager.GetUserAsync(HttpContext.User);
            var currentUserRoleId = _db.UserRoles.Where(r => r.UserId == currentUser.Id).Select(r => r.RoleId).FirstOrDefault();
            var currentUserRole = _db.Roles.Where(r => r.Id == currentUserRoleId).Select(r => r.Name).FirstOrDefault();

            return currentUser;

        }

        /// <summary>Gets the current user role name.</summary>
        private async Task<string> GetCurrentUserRoleNameAsync()
        {
            ApplicationUser currentUser = await GetCurrentUserAsync();
            var currentUserRoleId = _db.UserRoles.Where(r => r.UserId == currentUser.Id).Select(r => r.RoleId).FirstOrDefault();
            var currentUserRole = _db.Roles.Where(r => r.Id == currentUserRoleId).Select(r => r.Name).FirstOrDefault();

            return currentUserRole;

        }

        /// <summary>Builds the URL for Index methods.</summary>
        /// <param name="searchTitle">Title of the searched request.</param>
        /// <param name="searchClassification">Category of the searched request.</param>
        /// <param name="searchTeam">Team assigned to the searched request.</param>
        /// <param name="searchRequestor">Requestor of the searched request.</param>
        /// <param name="searchAssignee">Admin assigned to searched request.</param>
        /// <param name="searchStatus">Status of the searched request.</param>
        /// <param name="searchSubmitDateFrom">Starting date of submission of the searched request.</param>
        /// <param name="searchSubmitDateTo">Ending date of submission of the searched request.</param>
        private string BuildIndexUrl (string indexActionName, string searchTitle, string searchClassification, string searchTeam, string searchRequestor, string searchAssignee, string searchStatus, string searchSubmitDateFrom, string searchSubmitDateTo)
        {
            StringBuilder param = new StringBuilder();



            param.Append($"/RequestConfig/Requests/{indexActionName}?page=:");
            param.Append("&searchTitle=");
            if (searchTitle != null)
            {
                param.Append(searchTitle);
            }
            param.Append("&searchClassification=");
            if (searchClassification != null)
            {
                param.Append(searchClassification);
            }
            param.Append("&searchTeam=");
            if (searchTeam != null)
            {
                param.Append(searchTeam);
            }
            param.Append("&searchRequestor=");
            if (searchRequestor != null)
            {
                param.Append(searchRequestor);
            }
            param.Append("&searchAssignee=");
            if (searchAssignee != null)
            {
                param.Append(searchAssignee);
            }
            param.Append("&searchStatus=");
            if (searchStatus != null)
            {
                param.Append(searchStatus);
            }
            param.Append("&searchSubmitDateFrom=");
            if (searchSubmitDateFrom != null)
            {
                param.Append(searchSubmitDateFrom);
            }
            param.Append("&searchSubmitDateTo=");
            if (searchSubmitDateTo != null)
            {
                param.Append(searchSubmitDateTo);
            }

            return param.ToString();

        }


    }
}
