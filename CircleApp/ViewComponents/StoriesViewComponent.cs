using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.ViewComponents
{
    public class StoriesViewComponent(AppDbContext context) : ViewComponent
    {
        private readonly AppDbContext _context = context;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int userId = 1;
            var stories = await _context.Stories
                            .Include(s => s.User)
                            .Where(s => s.UserId == userId && s.DateCreated >= DateTime.UtcNow.AddHours(-24))
                            .OrderByDescending(s=> s.DateCreated)
                            .ToListAsync();

            return View(stories);
        }
        
    }
}