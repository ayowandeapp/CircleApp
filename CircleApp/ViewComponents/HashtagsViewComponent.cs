using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.ViewComponents
{
    public class HashtagsViewComponent(AppDbContext context) : ViewComponent
    {
        private readonly AppDbContext _context = context;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var hashtags = await _context.Hashtags
                            .OrderByDescending(s => s.Count)
                            .Take(3)
                            .ToListAsync();

            return View(hashtags);
        }
        
    }
}