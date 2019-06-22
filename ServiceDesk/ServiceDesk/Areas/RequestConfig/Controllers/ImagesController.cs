using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Data;
using ServiceDesk.Models;
using ServiceDesk.Models.ViewModels;

namespace ServiceDesk.Areas.RequestConfig.Controllers
{


    /// <summary>Provides actions for managing <see cref="Image"/> objects.</summary>
    [Area("RequestConfig")]
    [Authorize]
    public class ImagesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly HostingEnvironment _hostingEnvironment;

        /// <summary>Initializes a new instance of the <see cref="ImagesController"/> class.</summary>
        /// <param name="db">The database used for dependency injection.</param>
        /// <param name="hostingEnvironment">The <see cref="HostingEnvironment"/> instance used for dependency injection.</param>
        public ImagesController(ApplicationDbContext db, HostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }



        /// <summary>Creates <see cref="Image"/> object from a file uploaded by the user, passes it to <see cref="ImagesAssignedToRequest"/> object and save it in database.</summary>
        /// <param name="requestVM">Instance of <see cref="RequestViewModel"/> provided by View.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequestViewModel requestVM)
        {

            if (ModelState.IsValid)
            {

                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var requestFromDb = _db.Requests.Find(requestVM.Request.Id);



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

                        RequestId = requestVM.Request.Id,
                        Request = await _db.Requests.FindAsync(requestVM.Request.Id),
                        Image = image

                    };

                    await _db.ImagesAssignedToRequest.AddAsync(imageAssignedToRequest);

                }

                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Requests", new { id = requestVM.Request.Id });
            }

            return View();
        }

    }
}