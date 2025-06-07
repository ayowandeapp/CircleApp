using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CircleApp.Helpers.Constants;
using CircleApp.Models;
using CircleApp.ViewModels.Auth;
using CircleApp.ViewModels.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CircleApp.Controllers
{
    // [Route("Auth")]
    public class AuthenticationController(
        UserManager<User> userManager,
        SignInManager<User> signInManager
        ) : Controller
    {
        public readonly UserManager<User> _userManager = userManager;
        public readonly SignInManager<User> _signInManager = signInManager;

        public async Task<IActionResult> Login()
        {
            return View();
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }
            var newUser = new User()
            {
                Fullname = $"{registerVM.FirstName} {registerVM.LastName}",
                Email = registerVM.Email,
                UserName = registerVM.Email
            };
            var user = await _userManager.FindByEmailAsync(registerVM.Email);
            if (user != null)
            {
                ModelState.AddModelError("Email", "Email already exists");
                return View(registerVM);
            }
            var result = await _userManager.CreateAsync(newUser, registerVM.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, AppRoles.User);

                await _userManager.AddClaimAsync(newUser, new Claim("FullName", newUser.Fullname));
                
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(registerVM);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }

            var user = await _userManager.FindByEmailAsync(loginVM.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid Credentials");
                return View(loginVM);
            }
            
            var existingClaims = await _userManager.GetClaimsAsync(user);
            if (!existingClaims.Any(c => c.Type == "FullName"))
            { 
                await _userManager.AddClaimAsync(user, new Claim("FullName", user.Fullname));  
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, loginVM.Password, false, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            
            ModelState.AddModelError("", "Invalid Credentials");
            return View(loginVM);
            
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(PasswordVM passwordVM)
        {
            if (passwordVM.NewPassword != passwordVM.ConfirmPassword)
            {
                TempData["PasswordError"] = "Password do not match";
                TempData["ActiveTab"] = "Password";
                return RedirectToAction("Index", "Settings");
            }

            var user = await _userManager.GetUserAsync(User);
            var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, passwordVM.CurrentPassword);
            if (!isCurrentPasswordValid)
            {
                TempData["PasswordError"] = "Current Password is invalid";
                TempData["ActiveTab"] = "Password";
                return RedirectToAction("Index", "Settings");
            }
            var result = await _userManager.ChangePasswordAsync(user, passwordVM.CurrentPassword, passwordVM.NewPassword);
            if (result.Succeeded)
            {
                TempData["PasswordSuccess"] = "Password updated successfully";
                TempData["ActiveTab"] = "Password";

                await _signInManager.RefreshSignInAsync(user);
                
            }
            return RedirectToAction("Index", "Settings");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileVM profileVM)
        {
            var authUser = await _userManager.GetUserAsync(User);
            if(authUser == null) return RedirectToAction("Index", "Settings"); 
            authUser.Fullname = profileVM.Fullname;
            authUser.UserName = profileVM.Username;
            authUser.Bio = profileVM.Bio;

            var result = await _userManager.UpdateAsync(authUser);
            if (!result.Succeeded)
            {
                TempData["UserProfileError"] = "user Profile cannot be updated";
                TempData["ActiveTab"] = "Profile";
                
            }
            await _signInManager.RefreshSignInAsync(authUser);
            return RedirectToAction("Index", "Settings");

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}