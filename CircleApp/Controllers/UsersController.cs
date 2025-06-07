using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Controllers.Base;
using CircleApp.Models;
using CircleApp.Services;
using CircleApp.ViewModels.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CircleApp.Controllers
{
    // [Route("[controller]")]
    public class UsersController(IUserService userService, UserManager<User> userManager) : BaseController
    {
        private readonly IUserService _userService = userService;
        private readonly UserManager<User> _userManager = userManager;
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(string userId)
        {
            var userPosts = await _userService.GetUserPosts(int.Parse(userId));
            var user = await _userManager.FindByIdAsync(userId);
            var data = new GetUserProfileVM()
            {
                User = user,
                Posts = userPosts
            };

            return View(data);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}