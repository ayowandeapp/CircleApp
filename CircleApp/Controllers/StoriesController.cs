using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Data;
using CircleApp.Models;
using CircleApp.ViewModels.Stories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CircleApp.Controllers
{
    // [Route("[controller]")]
    public class StoriesController : Controller
    {
        private readonly ILogger<StoriesController> _logger;
        private readonly AppDbContext _context;

        public StoriesController(ILogger<StoriesController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _context = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            int userId = 1;
            var stories = await _context.Stories
                            .Include(s => s.User)
                            .Where(s => s.UserId == userId)
                            .ToListAsync();

            return View(stories);
        }
        [HttpPost]
        public async Task<IActionResult> CreateStories(PostStoriesVM postStoriesVM)
        {
            int userId = 1;
            
            var newStory = new Story
            {
                ImageUrl = "",
                UserId = userId
            };
            //check and save the image
            if (postStoriesVM.Image != null && postStoriesVM.Image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (postStoriesVM.Image.ContentType.Contains("image"))
                {
                    string pathImages = Path.Combine(rootFolderPath, "images/stories");
                    Directory.CreateDirectory(pathImages);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(postStoriesVM.Image.FileName);
                    string filePath = Path.Combine(pathImages, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await postStoriesVM.Image.CopyToAsync(stream);
                    //set the url
                    newStory.ImageUrl = "/images/stories/" + fileName;
                }
            }
            await _context.Stories.AddAsync(newStory);
            await _context.SaveChangesAsync();

            //redirect to index page
            return RedirectToAction("Index");

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}