using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CircleApp.Services;
using CircleApp.ViewModels.Friends;
using Microsoft.AspNetCore.Mvc;

namespace CircleApp.ViewComponents
{
    public class SuggestedFriendsViewComponent(IFriendService friendService) : ViewComponent
    {
        public readonly IFriendService _friendService = friendService;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = ((ClaimsPrincipal)User).FindFirstValue(ClaimTypes.NameIdentifier);
            var suggestedFriends = await _friendService.GetSuggestedFriendsAsync(int.Parse(userId));
            var suggestedFriendsVM = suggestedFriends.Select(f => new UserWithFriendsCountVM
            {
                UserId = f.User.Id,
                FullName = f.User.Fullname,
                ProfilePictureUrl = f.User.ProfilePictureUrl,
                FriendsCount = f.FriendsCount
            }).ToList();
            return View(suggestedFriendsVM);
        }
        
    }
}