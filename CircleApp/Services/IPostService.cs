using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Models;

namespace CircleApp.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetPostsAsync(int userId);
        Task<List<Post>> GetFavoritedPostsAsync();
        Task<Post> CreatePostAsync(Post post);
        Task<Post?> RemovePostAsync(int postId);
        Task AddPostCommentAsync(Comment comment);
        Task RemovePostCommentAsync(int commentId);
        Task TogglePostLikeAsync(int postId);
        Task TogglePostVisibilityAsync(int postId);
        Task TogglePostFavoriteAsync(int postId);
        Task ReportPostAsync(int postId);
        Task<Post?> GetPostByIdAsync(int postId);

    }
}