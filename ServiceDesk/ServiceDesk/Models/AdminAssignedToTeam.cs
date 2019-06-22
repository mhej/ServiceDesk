using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Models
{

    /// <summary>Provides foreign keys of teams and admin users assigned to them.</summary>
    public class AdminAssignedToTeam
    {

        /// <summary>Gets or sets the identifier.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the team identifier.</summary>
        [Display(Name = "Team")]
        public int TeamId { get; set; }

        /// <summary>Gets or sets the team using its identifier as a foreign key.</summary>
        [ForeignKey("TeamId")]
        public virtual Team Team { get; set; }

        /// <summary>Gets or sets the user identifier.</summary>
        [Display(Name = "ApplicationUser")]
        public string ApplicationUserId { get; set; }

        /// <summary>Gets or sets the user using its identifier as a foreign key.</summary>
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
