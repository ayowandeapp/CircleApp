using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Data;
using CircleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.Services
{
    public class UserService(AppDbContext context) : IUserService
    {
        private readonly AppDbContext _context = context;
        public async Task<User?> GetUser(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<List<Post>> GetUserPosts(int userId)
        {
            return await _context.Posts
                .Where(p => p.DateDeleted == null)
                .Where(p => ( p.UserId == userId) && p.Reports.Count < 5)
                .Include(n => n.User)
                .Include(n => n.Likes)
                .Include(n => n.Comments).ThenInclude(c => c.User)
                .Include(n => n.Favorites.Where(f => f.UserId == userId))
                .Include(n => n.Reports.Where(r => r.UserId == userId))
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();
        }

        public async Task UpdateUserProfilePicture(int userId, string profilePictureUrl)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                user.ProfilePictureUrl = profilePictureUrl;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

            }
        }
    }
}