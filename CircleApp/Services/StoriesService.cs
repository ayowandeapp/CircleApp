using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Data;
using CircleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.Services
{
    public class StoriesService(AppDbContext appDbContext) : IStoriesService
    {
        private readonly AppDbContext _context = appDbContext;
        public async Task<Story> CreateStoryAsync(Story story)
        {
            await _context.Stories.AddAsync(story);
            await _context.SaveChangesAsync();
            return story;
        }

        public async Task<List<Story>> GetStories()
        {
            int userId = 1;
            var stories = await _context.Stories
                            .Include(s => s.User)
                            .Where(s => s.UserId == userId && s.DateCreated >= DateTime.UtcNow.AddHours(-24))
                            .OrderByDescending(s => s.DateCreated)
                            .ToListAsync();
            return stories;
        }
    }
}