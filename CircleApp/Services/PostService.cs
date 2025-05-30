using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Data;
using CircleApp.Data.Helpers;
using CircleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.Services
{
    public class PostService(AppDbContext context) : IPostService
    {
        private readonly AppDbContext _context = context;
        public async Task AddPostCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            return post;
        }

        public async Task<List<Post>> GetPostsAsync(int userId)
        {
            return await _context.Posts
                .Where(p => p.DateDeleted == null)
                .Where(p => (!p.IsPrivate || p.UserId == userId) && p.Reports.Count < 5)
                .Include(n => n.User)
                .Include(n => n.Likes)
                .Include(n => n.Comments).ThenInclude(c => c.User)
                .Include(n => n.Favorites.Where(f => f.UserId == userId))
                .Include(n => n.Reports.Where(r => r.UserId == userId))
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();
        }

        public async Task<Post?> RemovePostAsync(int postId)
        {
            int userId = 1;

            var post = await _context.Posts.FirstOrDefaultAsync(
                p => p.Id == postId && p.UserId == userId
            );
            if (post != null)
            {
                post.SoftDelete();
                _context.Posts.Update(post);

            }
            await _context.SaveChangesAsync();

            return post;
        }

        public async Task RemovePostCommentAsync(int commentId)
        {
            int userId = 1;
            var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment == null || comment.UserId != userId)
            {
                // _logger.LogWarning($"User {userId} attempted to delete comment {comment?.Id} owned by {comment?.UserId}");
                return;

            }
            _context.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task ReportPostAsync(int postId)
        {            
            int userId = 1;
            Report newReport = new()
            {
                UserId = userId,
                PostId = postId,

            };
            await _context.Reports.AddAsync(newReport);
            await _context.SaveChangesAsync();
        }

        public async Task TogglePostFavoriteAsync(int postId)
        {
            
            int userId = 1;
            var favorite = await _context.Favorites.Where(f => f.PostId == postId && f.UserId == userId).FirstOrDefaultAsync();
            if (favorite != null)
            {
                _context.Favorites.Remove(favorite);
            }
            else
            {
                await _context.Favorites.AddAsync(new Favorite()
                {
                    PostId = postId,
                    UserId = userId

                });
            }
            await _context.SaveChangesAsync();
        }

        public async Task TogglePostLikeAsync(int postId)
        {            
            int userId = 1;
            var like = await _context.Likes
                .Where(l => l.PostId == postId && l.UserId == userId)
                .FirstOrDefaultAsync();
            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newLike = new Like()
                {
                    PostId = postId,
                    UserId = userId
                };
                await _context.AddAsync(newLike);
                await _context.SaveChangesAsync();
        }
        }

        public async Task TogglePostVisibilityAsync(int postId)
        {
            int userId = 1;
            var post = await _context.Posts
                .Where(p => p.Id == postId && p.UserId == userId)
                .FirstOrDefaultAsync();
            if (post != null)
            {
                post.IsPrivate = !post.IsPrivate;
                _context.Posts.Update(post);
                await _context.SaveChangesAsync();
            }
        }
    }
}