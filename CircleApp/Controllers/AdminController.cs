using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Helpers.Constants;
using CircleApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CircleApp.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class AdminController(IAdminService adminService) : Controller
    {
        private readonly IAdminService _adminService = adminService;
        public async Task<IActionResult> Index()
        {
            var reportedPosts = await _adminService.GetReportedPosts();

            return View(reportedPosts);
        }

        [HttpPost]
        public async Task<IActionResult> RejectReport(int postId)
        {
            await _adminService.RejectReport(postId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ApproveReport(int postId)
        {
            await _adminService.ApproveReport(postId);

            return RedirectToAction("Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}