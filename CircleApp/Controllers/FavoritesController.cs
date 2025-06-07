using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Controllers.Base;
using CircleApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CircleApp.Controllers
{
    [Authorize]
    public class FavoritesController(IPostService postService) : BaseController
    {
        public readonly IPostService _postService = postService;

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if(userId == null) return RedirectToLogin();

            var posts = await _postService.GetFavoritedPostsAsync(userId.Value);

            return View(posts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}