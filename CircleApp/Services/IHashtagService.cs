using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircleApp.Services
{
    public interface IHashtagService
    {
        Task ProcessHashtagsForNewPostAsync(string content);
        Task ProcessHashtagsForRemovedPostAsync(string content);
    }
}