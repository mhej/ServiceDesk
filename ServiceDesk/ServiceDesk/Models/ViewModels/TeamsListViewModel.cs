using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Models.ViewModels
{
    /// <summary>Provides view model for Team Views and Controller.</summary>
    public class TeamsListViewModel
    {
        /// <summary>Gets or sets the list of teams.</summary>
        public List<Team> Teams { get; set; }
        /// <summary>Gets or sets the name of the searched team.</summary>
        public string SearchName { get; set; }
        /// <summary>Gets or sets data required for pagination in the Index View.</summary>
        public PagingInfo PagingInfo { get; set; }
    }
}
