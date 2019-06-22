using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDesk.Models.ViewModels
{
    /// <summary>Provides view model for ChangePassword Views and Controller.</summary>
    public class ChangePasswordViewModel
    {
        /// <summary>Gets or sets the old password.</summary>
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        /// <summary>Gets or sets the new password.</summary>
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        /// <summary>Gets or sets the confirmed new password.</summary>
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
