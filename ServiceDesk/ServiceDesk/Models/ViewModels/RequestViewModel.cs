using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Models.ViewModels
{
    /// <summary>Provides view model for Request Views and Controller.</summary>
    public class RequestViewModel
    {
        /// <summary>Gets or sets the request.</summary>
        public Request Request { get; set; }

        /// <summary>Gets or sets collection of the teams.</summary>
        public IEnumerable<Team> Teams { get; set; }

        /// <summary>Gets or sets collection of the classifications.</summary>
        public IEnumerable <Classification> Classifications { get; set; }
        /// <summary>Gets or sets collection of the assignees.</summary>
        public IEnumerable<ApplicationUser> Assignees { get; set; }
        /// <summary>Gets or sets collection of the request notes.</summary>
        public IEnumerable<RequestNote> RequestNotes { get; set; }
        /// <summary>Gets or sets the request note.</summary>
        public RequestNote RequestNote { get; set; }
        /// <summary>Gets or sets collection of the images.</summary>
        public IEnumerable<Image> Images { get; set; }
        /// <summary>Gets or sets the image.</summary>
        public Image Image { get; set; }

    }
}
