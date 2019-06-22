using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Models.ViewModels
{
    /// <summary>Provides view model for Login Views and Controller.</summary>
    public class LoginViewModel
    {
        /// <summary>Gets or sets the name of the user.</summary>
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        /// <summary>Gets or sets the password.</summary>
        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
