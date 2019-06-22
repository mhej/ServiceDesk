using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Models.ViewModels
{
    /// <summary>Provides view model for TeamConfigClassifications Views and Controller.</summary>
    public class TeamConfigClassificationsViewModel
    {
        /// <summary>Gets or sets the team.</summary>
        public Team Team { get; set; }
        /// <summary>Gets or sets the list of classifications assigned to the specified team.</summary>
        public List<Classification> ClassificationsAssignedToTeam { get; set; }
        /// <summary>Gets or sets the list of all classifications.</summary>
        public List<Classification> AllClassifications { get; set; }

    }
}
