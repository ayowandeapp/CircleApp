using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Enums;
using CircleApp.Services;
using CircleApp.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CircleApp.Controllers
{
    // [Route("[controller]")]
    public class SettingsController(IUserService userService, IFilesService filesService) : Controller
    {
        public readonly IUserService _userService = userService;
        private readonly IFilesService _filesService = filesService;
        public async Task<IActionResult> Index()
        {
            int userId = 1;
            var user = await _userService.GetUser(userId);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(ProfilePictureVM profilePictureVM)
        {
            int userId = 1;
            var imagePath = await _filesService.UploadImageAsync(profilePictureVM.ProfilePictureImage, ImageFileTypeEnum.ProfileImage);

            await _userService.UpdateUserProfilePicture(userId, imagePath);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileVM profileVM)
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePassword(PasswordVM passwordVM)
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