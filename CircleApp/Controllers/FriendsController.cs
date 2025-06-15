using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Controllers.Base;
using CircleApp.Data.Helpers.Constants;
using CircleApp.Services;
using CircleApp.ViewModels.Friends;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CircleApp.Controllers
{
    public class FriendsController(
        IFriendService friendService,
        INotificationService notificationService
        ) : BaseController
    {
        public readonly IFriendService _friendService = friendService;
        public readonly INotificationService _notificationService = notificationService;
        public async Task<IActionResult> Index()
        {
            var userId = this.GetUserId();
            if (userId == null) return RedirectToLogin();

            var data = new FriendshipVM
            {
                Friends = await _friendService.GetFriends(userId.Value),
                FriendRequestsSent = await _friendService.GetSentFriendRequest(userId.Value),
                FriendRequestsReceived = await _friendService.GetReceivedFriendRequest(userId.Value)
            };
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(int receiverId)
        {
            var userId = this.GetUserId();
            var userName = GetUserFullName();
            if (userId == null) return RedirectToLogin();
            await _friendService.SendRequest(userId.Value, receiverId);

            await _notificationService.AddNotificationAsync(
                receiverId, userName, NotificationType.FriendRequest
                );
            return RedirectToAction("Index", "Home");

        }


        [HttpPost]
        public async Task<IActionResult> UpdateFriendRequest(FriendRequestUpdateVM friendRequestUpdateVM)
        {
            var userId = this.GetUserId();
            if (userId == null) return RedirectToLogin();
            var request = await _friendService.UpdateRequest(friendRequestUpdateVM.RequestId, friendRequestUpdateVM.Status);
            if (friendRequestUpdateVM.Status == FriendshipStatus.Accepted)
            {
                var userName = GetUserFullName();
                await _notificationService.AddNotificationAsync(
                    request.SenderId, userName, NotificationType.FriendRequestApproved
                    );
            }
            return RedirectToAction("Index", "Home");

        }
        
        [HttpPost]
        public async Task<IActionResult> RemoveFriend(int friendshipId)
        {
            var userId = this.GetUserId();
            if (userId == null) return RedirectToLogin();
            await _friendService.RemoveFriend(friendshipId);
            return RedirectToAction("Index", "Home");

        }
    }
}