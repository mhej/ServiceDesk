using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Models
{
    /// <summary>Represents an application user. Derives from IdentityUser class.</summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>Gets or sets the name.</summary>
        [Required]
        public string Name { get; set; }

    }
}
