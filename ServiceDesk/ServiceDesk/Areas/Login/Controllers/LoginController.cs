using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Models.ViewModels;

namespace ServiceDesk.Areas.Login.Controllers
{


    /// <summary>Provides action for log in and log out to the application.</summary>
    [Area("Login")]
    public class LoginController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>Initializes a new instance of the <see cref="LoginController"/> class.</summary>
        /// <param name="signInManager"><see cref="SignInManager{TUser}"/> instance used for dependency injection.</param>
        /// <param name="userManager"><see cref="UserManager{TUser}"/> instance used for dependency injection.</param>
        public LoginController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        /// <summary>Returns View for log in.</summary>
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>Logs in an Application User.</summary>
        /// <param name="loginVM">Instance of <see cref="LoginViewModel"/> provided by View.</param>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var user = await _userManager.FindByNameAsync(loginVM.UserName);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Requests", new { area = "RequestConfig" });
                }
            }

            ModelState.AddModelError("", "User name or password is invalid");

            return View(loginVM);
        }

        /// <summary>Logs out an Application User.</summary>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

    }
}