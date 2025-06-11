using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Dto;
using CircleApp.Models;

namespace CircleApp.Services
{
    public interface IFriendService
    {
        Task<bool> SendRequest(int senderId, int receriverId);
        Task<bool> UpdateRequest(int requestId, string status);
        Task<bool> RemoveFriend(int friendshipId);
        Task<List<UserWithFriendsCountDto>> GetSuggestedFriendsAsync(int userId);
        Task<List<FriendRequest>> GetSentFriendRequest(int userId);
        Task<List<FriendRequest>> GetReceivedFriendRequest(int userId);
        Task<List<Friendship>> GetFriends(int userId);
    }
}