using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Data;
using CircleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.Services
{
    public class AdminService(AppDbContext appDbContext) : IAdminService
    {
        private readonly AppDbContext _context = appDbContext;

        public async Task ApproveReport(int postId)
        {
            var postDb = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (postDb != null)
            {
                postDb.SoftDelete();
                _context.Posts.Update(postDb);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Post>> GetReportedPosts()
        {
            // var posts = await _context.Posts
            //                 .Include(n => n.Reports)
            //                 .Where(n => n.Reports.Count > 5 && !n.IsDeleted)
            //                 .ToListAsync();
            return await _context.Posts
                            .Where(p => p.NrOfReports > 5 && p.DateDeleted == null)
                            .Include(p => p.User)
                            .ToListAsync();

        }

        public async Task RejectReport(int postId)
        {
            var postDb = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (postDb != null)
            {
                postDb.NrOfReports = 0;
                _context.Posts.Update(postDb);
                await _context.SaveChangesAsync();

                var reports = postDb.Reports;
                // var rs = await _context.Reports.Where(r => r.PostId == postId).ToListAsync();
                if (reports.Count != 0)
                {
                    _context.Reports.RemoveRange(reports);
                    await _context.SaveChangesAsync();
                }

            }
        }
    }
}