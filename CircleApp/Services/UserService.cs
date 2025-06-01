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