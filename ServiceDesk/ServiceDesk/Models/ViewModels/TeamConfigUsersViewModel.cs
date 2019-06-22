using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Models.ViewModels
{
    /// <summary>Provides view model for TeamConfigUsers Views and Controller.</summary>
    public class TeamConfigUsersViewModel
    {
        /// <summary>Gets or sets the team.</summary>
        public Team Team { get; set; }
        /// <summary>Gets or sets the list of admin users assigned to the specified team.</summary>
        public List<ApplicationUser> AdminsAssignedToTeam { get; set; }
        /// <summary>Gets or sets the list of all admin users.</summary>
        public List<ApplicationUser> AllAdmins { get; set; }

    }
}
