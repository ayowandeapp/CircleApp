using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Models;

namespace CircleApp.Services
{
    public interface IStoriesService
    {
        Task<List<Story>> GetStories();
        Task<Story> CreateStoryAsync(Story story);
    }
}