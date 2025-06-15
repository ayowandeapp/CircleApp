using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Data;
using CircleApp.Data.Helpers.Constants;
using CircleApp.Hubs;
using CircleApp.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.Services
{
    public class NotificationService(
        AppDbContext appDbContext,
        IHubContext<NotificationHub> hubContext
        ) : INotificationService
    {
        private readonly AppDbContext _context = appDbContext;
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;
        public async Task AddNotificationAsync(
            int userId,
            string userFullName,
            string notificationType,
            int? postId = null
        )
        {
            var not = new Notification()
            {
                UserId = userId,
                Message = GetPostMessage(notificationType, userFullName),
                Type = notificationType,
                PostId = postId
            };
            await _context.Notifications.AddAsync(not);
            await _context.SaveChangesAsync();

            
        var notCount = await GetUnreadNotificationCount(userId);

        await _hubContext.Clients.User(userId.ToString())
            .SendAsync("ReceiveNotification", notCount);

        }

        private static string GetPostMessage(string notificationType, string userFullName)
        {
            string msg = notificationType switch
            {
                NotificationType.Like => $"{userFullName} liked your post",
                NotificationType.Favorite => $"{userFullName} favorited your post",
                NotificationType.Comment => $"{userFullName} added a comment to your post",
                NotificationType.FriendRequest => $"{userFullName} sent you a friend request",
                NotificationType.FriendRequestApproved => $"{userFullName} is now your friemd",
                _ => ""
            };
            return msg;
        }

        public async Task<int> GetUnreadNotificationCount(int userId)
        {
            return await _context.Notifications.Where(n => n.UserId == userId && !n.IsRead).CountAsync();
        }

        public async Task<List<Notification>> GetNotifications(int userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderBy(n => n.IsRead)
                .ThenByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task SetNotificaionAsRead(int notificationId)
        {
            var notificationDb = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == notificationId);
            if (notificationDb != null)
            {
                notificationDb.IsRead = true;
                notificationDb.UpdatedAt = DateTime.UtcNow;
                _context.Update(notificationDb);
                await _context.SaveChangesAsync();          
            }
        }
    }
}