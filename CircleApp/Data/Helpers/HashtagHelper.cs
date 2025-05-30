using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CircleApp.Data.Helpers
{
    public static class HashtagHelper
    {
        // search a post to find hashtags
        public static List<string> GetHashtags(string postText)
        {
            var hashtagPattern = new Regex(@"#\w+"); //start with # tag followed by words
            var matches = hashtagPattern.Matches(postText)//returns a list of match words
                            .Select(match => match.Value.TrimEnd('.', ',', '!', '?').ToLower()) //remove the ,.!? at the end of the tags
                            .Distinct()
                            .ToList();
            return matches;
        }
        
    }
}