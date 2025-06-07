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
        Task<List<Post>> GetFavoritedPostsAsync(int userId);
        Task<Post> CreatePostAsync(Post post);
        Task<Post?> RemovePostAsync(int postId, int userId);
        Task AddPostCommentAsync(Comment comment);
        Task RemovePostCommentAsync(int commentId, int userId);
        Task TogglePostLikeAsync(int postId, int userId);
        Task TogglePostVisibilityAsync(int postId, int userId);
        Task TogglePostFavoriteAsync(int postId, int userId);
        Task ReportPostAsync(int postId, int userId);
        Task<Post?> GetPostByIdAsync(int postId);

    }
}