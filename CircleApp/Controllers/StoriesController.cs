using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Controllers.Base;
using CircleApp.Data;
using CircleApp.Enums;
using CircleApp.Models;
using CircleApp.Services;
using CircleApp.ViewModels.Stories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CircleApp.Controllers
{
    // [Route("[controller]")]
    
    [Authorize]
    public class StoriesController : BaseController
    {
        private readonly ILogger<StoriesController> _logger;
        private readonly IStoriesService _storiesService;
        private readonly IFilesService _filesService;

        public StoriesController(
            IFilesService filesService,
            ILogger<StoriesController> logger, IStoriesService storiesService)
        {
            _logger = logger;
            _storiesService = storiesService;
            _filesService = filesService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateStories(PostStoriesVM postStoriesVM)
        {
            var userId = GetUserId();
            if(userId == null) return RedirectToLogin();

            var newStory = new Story
            {
                ImageUrl = "",
                UserId = userId.Value
            };
            newStory.ImageUrl = await _filesService.UploadImageAsync(postStoriesVM.Image, ImageFileTypeEnum.StoryImage);

            await _storiesService.CreateStoryAsync(newStory);

            //redirect to index page
            return RedirectToAction("Index", "Home");

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}