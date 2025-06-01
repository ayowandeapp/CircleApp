using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Models;

namespace CircleApp.Services
{
    public interface IUserService
    {
        Task<User?> GetUser(int userId);
        Task UpdateUserProfilePicture(int userId, string profilePictureUrl);
    }
}