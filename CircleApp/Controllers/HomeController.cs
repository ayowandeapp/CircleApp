using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CircleApp.Models;
using CircleApp.Data;
using Microsoft.EntityFrameworkCore;
using CircleApp.ViewModels.Home;
using CircleApp.Data.Helpers;
using CircleApp.Services;
using CircleApp.Enums;

namespace CircleApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _appDbcontext;
    private readonly IPostService _postService;
    private readonly IHashtagService _hashtagService;
    private readonly IFilesService _filesService;

    public HomeController(IPostService postService, IHashtagService hashtagService,
            IFilesService filesService,
        ILogger<HomeController> logger, AppDbContext appDbContext)
    {
        _logger = logger;
        _appDbcontext = appDbContext;
        _postService = postService;
        _hashtagService = hashtagService;
        _filesService = filesService;
    }

    public async Task<IActionResult> Index()
    {
        int userId = 1;
        var allPosts = await _postService.GetPostsAsync(userId);

        return View(allPosts);
    }

    public async Task<IActionResult> Details(int PostId)
    {
        var post = await _postService.GetPostByIdAsync(PostId);

        return View(post);
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
        newPost.ImageUrl = await _filesService.UploadImageAsync(post.Image, ImageFileTypeEnum.PostImage);

        newPost = await _postService.CreatePostAsync(newPost);

        if (newPost.Content != null)
        {
            await _hashtagService.ProcessHashtagsForNewPostAsync(newPost.Content);
            
        }
        

        //redirect to index page
        return RedirectToAction("Index");

    }

    [HttpPost]
    public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
    {
        await _postService.TogglePostLikeAsync(postLikeVM.PostId);
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
        await _postService.AddPostCommentAsync(newComment);

        return RedirectToAction("Index");

    }

    [HttpPost]
    public async Task<IActionResult> RemovePostComment(RemoveCommentVM removeCommentVM)
    {
        await _postService.RemovePostCommentAsync(removeCommentVM.CommentId);

        return RedirectToAction("Index");

    }

    [HttpPost]
    public async Task<IActionResult> ToggleFavoritePosts(PostFavoriteVM postFavoriteVM)
    {
        await _postService.TogglePostFavoriteAsync(postFavoriteVM.PostId);
        return RedirectToAction("Index");

    }

    [HttpPost]
    public async Task<IActionResult> TogglePostVisibility(PostVisibilityVM postVisibilityVM)
    {
        await _postService.TogglePostVisibilityAsync(postVisibilityVM.PostId);
        
        return RedirectToAction("Index");

    }
    
    
    [HttpPost]
    public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
    {
        await _postService.ReportPostAsync(postReportVM.PostId);

        return RedirectToAction("Index");

    }
 
    
    [HttpPost]
    public async Task<IActionResult> PostDelete(PostDeleteVM postDeleteVM)
    {
        int userId = 1;

        var post = await _postService.RemovePostAsync(postDeleteVM.PostId);

        if (post?.Content != null)
        {
            await _hashtagService.ProcessHashtagsForRemovedPostAsync(post.Content);
        }


        return RedirectToAction("Index");

    }
    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}
