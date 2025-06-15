using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Models;

namespace CircleApp.Services
{
    public interface INotificationService
    {
        Task AddNotificationAsync(int userId, string userFullName, string notificationType, int? postId = null);
        Task<int> GetUnreadNotificationCount(int userId);
        Task<List<Notification>> GetNotifications(int userId);
        Task SetNotificaionAsRead(int notificationId);
    }
}