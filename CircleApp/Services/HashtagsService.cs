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
    public class HashtagsService(AppDbContext context) : IHashtagService
    {
        private readonly AppDbContext _context = context;

        public async Task ProcessHashtagsForNewPostAsync(string content)
        {            
            //find and score hashtags
            var postHashtags = HashtagHelper.GetHashtags(content);
            foreach (var tags in postHashtags)
            {
                var tagDb = await _context.Hashtags.FirstOrDefaultAsync(h => h.Name == tags);

                if (tagDb != null)
                {
                    tagDb.Count += 1;
                    tagDb.DateUpdated = DateTime.UtcNow;
                    
                    _context.Hashtags.Update(tagDb);
                }
                else
                {
                    await _context.Hashtags.AddAsync(new Hashtag
                    {
                        Name = tags,
                        Count = 1

                    });
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task ProcessHashtagsForRemovedPostAsync(string content)
        {
            
            //decrease the hashtags
            var tags = HashtagHelper.GetHashtags(content);
            foreach (var tag in tags)
            {
                var tagDb = await _context.Hashtags.FirstOrDefaultAsync(
                    p => p.Name == tag
                );
                if (tagDb != null)
                {
                    tagDb.Count -= 1;
                    _context.Hashtags.Update(tagDb);

                }
            }
            await _context.SaveChangesAsync();
        }
    }
}