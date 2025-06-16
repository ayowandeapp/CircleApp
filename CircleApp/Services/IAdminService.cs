using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Models;

namespace CircleApp.Services
{
    public interface IAdminService
    {
        Task<List<Post>> GetReportedPosts();
        Task ApproveReport(int postId);
        Task RejectReport(int postId);

    }
}