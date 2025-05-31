using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Data;
using CircleApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.ViewComponents
{
    public class StoriesViewComponent(IStoriesService storiesService) : ViewComponent
    {
        private readonly IStoriesService _storiesService = storiesService;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var stories = await _storiesService.GetStories();

            return View(stories);
        }
        
    }
}