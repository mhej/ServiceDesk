using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Models;
using ServiceDesk.Models.ViewModels;

namespace ServiceDesk.Areas.Login.Controllers
{
    /// <summary>Provides action for password managing.</summary>
    [Area("Login")]
    public class ChangePasswordController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>Initializes a new instance of the <see cref="ChangePasswordController"/> class.</summary>
        /// <param name="userManager"><see cref="UserManager{TUser}"/> instance used for dependency injection.</param>
        public ChangePasswordController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>Returns View for change of password.</summary>
        public IActionResult ChangePassword()
        {
            return View();
        }




        /// <summary>Changes the password.</summary>
        /// <param name="teamConfigUsersVM">Instance of <see cref="ChangePasswordViewModel"/> provided by View.</param>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordVM )
        {
 
            if (changePasswordVM.NewPassword != changePasswordVM.ConfirmPassword)
            {
                ModelState.AddModelError("", "Please confirm a new password.");
            }

            ApplicationUser currentUser = (ApplicationUser)await _userManager.GetUserAsync(HttpContext.User);

            bool isOldPasswordCorrect = await _userManager.CheckPasswordAsync(currentUser, changePasswordVM.OldPassword);

            if (!isOldPasswordCorrect)
            {
                ModelState.AddModelError("", "Please enter correct old password");
            }

            if (!ModelState.IsValid)
            {
                return View(changePasswordVM);
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(currentUser, changePasswordVM.OldPassword, changePasswordVM.NewPassword);

            return RedirectToAction("Index", "Requests", new { area = "RequestConfig" });
        }


    }
}