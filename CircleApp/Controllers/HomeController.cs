using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CircleApp.Models;
using CircleApp.Data;
using Microsoft.EntityFrameworkCore;
using CircleApp.ViewModels.Home;
using CircleApp.Data.Helpers;

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
        int userId = 1;
        var allPosts = await _appDbcontext.Posts
                .Where(p => p.DateDeleted == null)
                .Where(p => (!p.IsPrivate || p.UserId == userId) && p.Reports.Count < 5)
                .Include(n => n.User)
                .Include(n => n.Likes)
                .Include(n => n.Comments).ThenInclude(c => c.User)
                .Include(n => n.Favorites.Where(f => f.UserId == userId))
                .Include(n => n.Reports.Where(r => r.UserId == userId))
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
                string pathImages = Path.Combine(rootFolderPath, "images/posts");
                Directory.CreateDirectory(pathImages);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.Image.FileName);
                string filePath = Path.Combine(pathImages, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await post.Image.CopyToAsync(stream);
                //set the url
                newPost.ImageUrl = "/images/posts/" + fileName;
            }
        }
        await _appDbcontext.Posts.AddAsync(newPost);
        await _appDbcontext.SaveChangesAsync();

        //find and score hashtags
        var postHashtags = HashtagHelper.GetHashtags(newPost.Content);
        foreach (var tags in postHashtags)
        {
            var tagDb = await _appDbcontext.Hashtags.FirstOrDefaultAsync(h => h.Name == tags);

            if (tagDb != null)
            {
                tagDb.Count += 1;
                tagDb.DateUpdated = DateTime.UtcNow;
                
                _appDbcontext.Hashtags.Update(tagDb);
            }
            else
            {
                await _appDbcontext.Hashtags.AddAsync(new Hashtag
                {
                    Name = tags,
                    Count = 1

                });
            }
        }
        await _appDbcontext.SaveChangesAsync();

        //redirect to index page
        return RedirectToAction("Index");

    }

    [HttpPost]
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

    [HttpPost]
    public async Task<IActionResult> AddPostComment(PostCommentVM postCommentVM)
    {
        int userId = 1;
        Comment newComment = new()
        {
            UserId = userId,
            PostId = postCommentVM.PostId,
            Content = postCommentVM.Content,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,

        };
        await _appDbcontext.Comments.AddAsync(newComment);
        await _appDbcontext.SaveChangesAsync();

        return RedirectToAction("Index");

    }

    [HttpPost]
    public async Task<IActionResult> RemovePostComment(RemoveCommentVM removeCommentVM)
    {
        int userId = 1;
        var comment = await _appDbcontext.Comments.FirstOrDefaultAsync(c => c.Id == removeCommentVM.CommentId);
        if (comment == null || comment.UserId != userId)
        {
            _logger.LogWarning($"User {userId} attempted to delete comment {comment?.Id} owned by {comment?.UserId}");
            return Forbid();
            // return RedirectToAction("Index");

        }
        _appDbcontext.Remove(comment);
        await _appDbcontext.SaveChangesAsync();

        return RedirectToAction("Index");

    }

    [HttpPost]
    public async Task<IActionResult> ToggleFavoritePosts(PostFavoriteVM postFavoriteVM)
    {
        int userId = 1;
        var favorite = await _appDbcontext.Favorites.Where(f => f.PostId == postFavoriteVM.PostId && f.UserId == userId).FirstOrDefaultAsync();
        if (favorite != null)
        {
            _appDbcontext.Favorites.Remove(favorite);
        }
        else
        {
            await _appDbcontext.Favorites.AddAsync(new Favorite()
            {
                PostId = postFavoriteVM.PostId,
                UserId = userId

            });
        }
        await _appDbcontext.SaveChangesAsync();
        return RedirectToAction("Index");

    }

    [HttpPost]
    public async Task<IActionResult> TogglePostVisibility(PostVisibilityVM postVisibilityVM)
    {
        int userId = 1;
        var post = await _appDbcontext.Posts
            .Where(p => p.Id == postVisibilityVM.PostId && p.UserId == userId)
            .FirstOrDefaultAsync();
        if (post != null)
        {
            post.IsPrivate = !post.IsPrivate;
            _appDbcontext.Posts.Update(post);
            await _appDbcontext.SaveChangesAsync();
        }
        return RedirectToAction("Index");

    }
    
    
    [HttpPost]
    public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
    {
        int userId = 1;
        Report newReport = new()
        {
            UserId = userId,
            PostId = postReportVM.PostId,

        };
        await _appDbcontext.Reports.AddAsync(newReport);
        await _appDbcontext.SaveChangesAsync();

        return RedirectToAction("Index");

    }
 
    
    [HttpPost]
    public async Task<IActionResult> PostDelete(PostDeleteVM postDeleteVM)
    {
        int userId = 1;
        var post = await _appDbcontext.Posts.FirstOrDefaultAsync(
            p => p.Id == postDeleteVM.PostId && p.UserId == userId
        );
        if (post != null)
        {
            post.SoftDelete();      
            _appDbcontext.Posts.Update(post);
            
            //decrease the hashtags
            var tags = HashtagHelper.GetHashtags(post.Content);
            foreach (var tag in tags)
            {
                var tagDb = await _appDbcontext.Hashtags.FirstOrDefaultAsync(
                    p => p.Name == tag
                );
                if (tagDb != null)
                {
                    tagDb.Count -= 1;
                    _appDbcontext.Hashtags.Update(tagDb);

                }
            }
        }
        await _appDbcontext.SaveChangesAsync();


        return RedirectToAction("Index");

    }
    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}
