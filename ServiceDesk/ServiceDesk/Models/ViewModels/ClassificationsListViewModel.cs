using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Models.ViewModels
{
    /// <summary>Provides view model for Classification Views and Controller.</summary>
    public class ClassificationsListViewModel
    {
        /// <summary>Gets or sets the list of classifications.</summary>
        public List<Classification> Classifications { get; set; }

        /// <summary>Gets or sets the name of the searched classification.</summary>
        public string SearchName { get; set; }

        /// <summary>Gets or sets data required for pagination in the Index View.</summary>
        public PagingInfo PagingInfo { get; set; }
    }
}
