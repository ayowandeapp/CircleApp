using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CircleApp.Controllers.Base;
using CircleApp.Enums;
using CircleApp.Helpers.Constants;
using CircleApp.Services;
using CircleApp.ViewModels.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CircleApp.Controllers
{
    // [Route("[controller]")]
    
    [Authorize(Roles = $"{AppRoles.User}, {AppRoles.Admin}")]
    public class SettingsController(IUserService userService, IFilesService filesService) : BaseController
    {
        public readonly IUserService _userService = userService;
        private readonly IFilesService _filesService = filesService;
        public async Task<IActionResult> Index()
        {
            var authUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userService.GetUser(int.Parse(authUser));
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(ProfilePictureVM profilePictureVM)
        {
            var userId = GetUserId();
            if(userId == null) return RedirectToLogin();
            var imagePath = await _filesService.UploadImageAsync(profilePictureVM.ProfilePictureImage, ImageFileTypeEnum.ProfileImage);

            await _userService.UpdateUserProfilePicture(userId.Value, imagePath);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileVM profileVM)
        {
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}