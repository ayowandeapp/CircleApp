using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CircleApp.Models;
using CircleApp.Data;
using Microsoft.EntityFrameworkCore;
using CircleApp.ViewModels.Home;

namespace CircleApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _appDbcontext;

    public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext)
    {
        _logger = logger;
        _appDbcontext = appDbContext;
    }

    public async Task<IActionResult> Index()
    {
        var allPosts = await _appDbcontext.Posts
                .Include(n => n.User)
                .Include(n=> n.Likes)
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();

        return View(allPosts);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePost(PostVM post)
    {
        Console.WriteLine(post.Content);
        //loggedin user
        int userId = 1;
        var newPost = new Post
        {
            Content = post.Content,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            ImageUrl = "",
            NrOfReports = 0,
            UserId = userId
        };
        //check and save the image
        if (post.Image != null && post.Image.Length > 0)
        {
            string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (post.Image.ContentType.Contains("image"))
            {
                string pathImages = Path.Combine(rootFolderPath, "images/uploaded");
                Directory.CreateDirectory(pathImages);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.Image.FileName);
                string filePath = Path.Combine(pathImages, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await post.Image.CopyToAsync(stream);
                //set the url
                newPost.ImageUrl = "/images/uploaded/" + fileName;
            }
        }
        await _appDbcontext.Posts.AddAsync(newPost);
        await _appDbcontext.SaveChangesAsync();

        //redirect to index page
        return RedirectToAction("Index");

    }

    public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
    {
        int userId = 1;
        var like = await _appDbcontext.Likes
            .Where(l => l.PostId == postLikeVM.PostId && l.UserId == userId)
            .FirstOrDefaultAsync();
        if (like != null)
        {
            _appDbcontext.Likes.Remove(like);
            await _appDbcontext.SaveChangesAsync();
        }
        else
        {
            var newLike = new Like()
            {
                PostId = postLikeVM.PostId,
                UserId = userId
            };
            await _appDbcontext.AddAsync(newLike);
            await _appDbcontext.SaveChangesAsync();
        }
        return RedirectToAction("Index");

    }


    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}
