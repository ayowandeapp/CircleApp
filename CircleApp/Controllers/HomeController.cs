using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CircleApp.Models;
using CircleApp.Data;
using Microsoft.EntityFrameworkCore;
using CircleApp.ViewModels.Home;
using CircleApp.Data.Helpers;
using CircleApp.Services;
using CircleApp.Enums;
using Microsoft.AspNetCore.Authorization;
using CircleApp.Controllers.Base;
using Microsoft.AspNetCore.SignalR;
using CircleApp.Hubs;
using CircleApp.Data.Helpers.Constants;
using CircleApp.Helpers.Constants;

namespace CircleApp.Controllers;

[Authorize(Roles = AppRoles.User)]
public class HomeController(
    IPostService postService,
    IHashtagService hashtagService,
    IFilesService filesService,
    ILogger<HomeController> logger,
    AppDbContext appDbContext,
    INotificationService notificationService
    ) : BaseController
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly AppDbContext _appDbcontext = appDbContext;
    private readonly IPostService _postService = postService;
    private readonly IHashtagService _hashtagService = hashtagService;
    private readonly IFilesService _filesService = filesService;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if(userId == null) return RedirectToLogin();
        var allPosts = await _postService.GetPostsAsync(userId.Value);

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
        var userId = GetUserId();
        if(userId == null) return RedirectToLogin();
        var newPost = new Post
        {
            Content = post.Content,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,
            ImageUrl = "",
            NrOfReports = 0,
            UserId = userId.Value
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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
    {
        var userId = GetUserId();
        if (userId == null) return RedirectToLogin();
        var res = await _postService.TogglePostLikeAsync(postLikeVM.PostId, userId.Value);

        var post = await _postService.GetPostByIdAsync(postLikeVM.PostId);

        if (res.SendNotification && userId.Value != post.UserId)
        {
            await _notificationService.AddNotificationAsync(
                post.UserId,
                GetUserFullName(),
                NotificationType.Like,
                post.Id
            );

        }

        return PartialView("Home/_Post", post);

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPostComment(PostCommentVM postCommentVM)
    {
        var userId = GetUserId();
        if (userId == null) return RedirectToLogin();
        Comment newComment = new()
        {
            UserId = userId.Value,
            PostId = postCommentVM.PostId,
            Content = postCommentVM.Content,
            DateCreated = DateTime.UtcNow,
            DateUpdated = DateTime.UtcNow,

        };
         await _postService.AddPostCommentAsync(newComment);
        var post = await _postService.GetPostByIdAsync(postCommentVM.PostId);

        if (userId.Value != post.UserId)
        {            
            await _notificationService.AddNotificationAsync(
                    post.UserId,
                    GetUserFullName(),
                    NotificationType.Comment,
                    post.Id
                );
        }
        return PartialView("Home/_post", post); 

        // return RedirectToAction("Index");

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemovePostComment(RemoveCommentVM removeCommentVM)
    {
        var userId = GetUserId();
        if(userId == null) return RedirectToLogin();
        await _postService.RemovePostCommentAsync(removeCommentVM.CommentId, userId.Value);

        var post = await _postService.GetPostByIdAsync(removeCommentVM.PostId);
        
        return PartialView("Home/_Post", post);

        // return RedirectToAction("Index");

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleFavoritePosts(PostFavoriteVM postFavoriteVM)
    {
        var userId = GetUserId();
        if (userId == null) return RedirectToLogin();
        var res = await _postService.TogglePostFavoriteAsync(postFavoriteVM.PostId, userId.Value);
        var post = await _postService.GetPostByIdAsync(postFavoriteVM.PostId);

        if (res.SendNotification && userId.Value != post.UserId)
        {
            await _notificationService.AddNotificationAsync(
                post.UserId,
                GetUserFullName(),
                NotificationType.Favorite,
                post.Id
            );

        }
        return PartialView("Home/_Post", post);

    }

    [HttpPost]
    public async Task<IActionResult> TogglePostVisibility(PostVisibilityVM postVisibilityVM)
    {
        var userId = GetUserId();
        if(userId == null) return RedirectToLogin();
        await _postService.TogglePostVisibilityAsync(postVisibilityVM.PostId, userId.Value);

        return RedirectToAction("Index");

    }


    [HttpPost]
    public async Task<IActionResult> AddPostReport(PostReportVM postReportVM)
    {
        var userId = GetUserId();
        if(userId == null) return RedirectToLogin();
        await _postService.ReportPostAsync(postReportVM.PostId, userId.Value);

        return RedirectToAction("Index");

    }


    [HttpPost]
    public async Task<IActionResult> PostDelete(PostDeleteVM postDeleteVM)
    {
        var userId = GetUserId();
        if(userId == null) return RedirectToLogin();
        var post = await _postService.RemovePostAsync(postDeleteVM.PostId, userId.Value);

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
