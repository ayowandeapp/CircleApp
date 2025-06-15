using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Controllers.Base;
using CircleApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CircleApp.Controllers
{
    public class NotificationsController(
        ILogger<NotificationsController> logger,
        INotificationService notificationService
        ) : BaseController
    {
        private readonly ILogger<NotificationsController> _logger = logger;
        private readonly INotificationService _notificationService = notificationService;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCount()
        {
            var userId = GetUserId();
            if (userId == null) RedirectToLogin();
            var count = await _notificationService.GetUnreadNotificationCount(userId.Value);
            return Json(count);
        }

        public async Task<IActionResult> GetNotifications()
        {
            var userId = GetUserId();
            if (userId == null) RedirectToLogin();
            var notifications = await _notificationService.GetNotifications(userId.Value);
            // return Json(notifications);
            return PartialView("Notifications/_Notification", notifications);
        }

        [HttpPost]
        public async Task<IActionResult> SetNotificationAsRead(int notificationId)
        {
            await _notificationService.SetNotificaionAsRead(notificationId);

            return RedirectToAction("GetNotifications");
        }

    }
}