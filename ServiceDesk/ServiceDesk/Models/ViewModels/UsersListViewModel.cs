using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Models.ViewModels
{
    /// <summary>Provides view model for AdminUser Views and Controller.</summary>
    public class UsersListViewModel
    {
        /// <summary>Gets or sets the list of application users.</summary>
        public List<ApplicationUser> Users { get; set; }
        /// <summary>Gets or sets collection of the user roles.</summary>
        public Dictionary<string, string> Roles { get; set; }
        /// <summary>Gets or sets the name of the searched user.</summary>
        public string SearchName { get; set; }
        /// <summary>Gets or sets the role name of the searched user.</summary>
        public string SearchRole { get; set; }
        /// <summary>Gets or sets data required for pagination in the Index View.</summary>
        public PagingInfo PagingInfo { get; set; }
    }
}
