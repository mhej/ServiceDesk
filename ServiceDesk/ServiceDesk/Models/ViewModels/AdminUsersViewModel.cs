using ServiceDesk.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Models.ViewModels
{
    /// <summary>Provides view model for AdminUsers Views and Controller.</summary>
    public class AdminUsersViewModel
    {
        /// <summary>Gets or sets the application user.</summary>
        public ApplicationUser ApplicationUser { get; set; }
        /// <summary>Gets or sets the application user role name.</summary>
        public string ApplicationUserRole { get; set; }
    }
}
