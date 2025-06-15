using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Data;
using CircleApp.Data.Helpers.Constants;
using CircleApp.Dto;
using CircleApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.Services
{
    public class FriendService(AppDbContext context) : IFriendService
    {
        public readonly AppDbContext _context = context;

        public async Task<List<Friendship>> GetFriends(int userId)
        {
            return await _context.Friendships
                            .Where(f => f.ReceiverId == userId || f.SenderId == userId)
                            .Include(r => r.Sender)
                            .Include(r => r.Receiver)
                            .ToListAsync();
        }

        public async Task<List<FriendRequest>> GetReceivedFriendRequest(int userId)
        {
            return await _context.FriendRequests
                                .Where(r => r.ReceiverId == userId && r.Status == FriendshipStatus.Pending)
                                .Include(r => r.Sender)
                                .Include(r => r.Receiver)
                                .ToListAsync();
        }

        public async Task<List<FriendRequest>> GetSentFriendRequest(int userId)
        {
            return await _context.FriendRequests
                                .Where(r => r.SenderId == userId && r.Status == FriendshipStatus.Pending)
                                .Include(r => r.Sender)
                                .Include(r => r.Receiver)
                                .ToListAsync();
        }

        public async Task<List<UserWithFriendsCountDto>> GetSuggestedFriendsAsync(int userId)
        {
            var existingFriendsIds = await _context.Friendships
                                        .Where(f => f.SenderId == userId || f.ReceiverId == userId)
                                        .Select(f => f.SenderId == userId ? f.ReceiverId : f.SenderId)
                                        .ToListAsync();
            //Pending requests
            var pendingRequestIds = await _context.FriendRequests
                                        .Where(f => (f.SenderId == userId || f.ReceiverId == userId) && f.Status == FriendshipStatus.Pending)
                                        .Select(f => f.SenderId == userId ? f.ReceiverId : f.SenderId)
                                        .ToListAsync();
            //suggested friends
            var suggestedFriends = await _context.Users
                                        .Where(u =>
                                            u.Id != userId &&
                                            !existingFriendsIds.Contains(u.Id) &&
                                            !pendingRequestIds.Contains(u.Id)
                                            ).Select(u => new UserWithFriendsCountDto
                                            {
                                                User = u,
                                                FriendsCount = _context.Friendships
                                                                    .Count(f => f.SenderId == u.Id || f.ReceiverId == u.Id)
                                            })
                                            .Take(5)
                                            .ToListAsync();

            return suggestedFriends;
        }

        public async Task<bool> RemoveFriend(int friendshipId)
        {
            var friendship = await _context.Friendships.FindAsync(friendshipId);
            if (friendship != null)
            {
                _context.Friendships.Remove(friendship);
                //find requests
                var requests = await _context.FriendRequests
                                    .Where(r => (r.SenderId == friendship.SenderId && r.ReceiverId == friendship.ReceiverId) ||
                                                 (r.SenderId == friendship.ReceiverId && r.ReceiverId == friendship.SenderId))
                                    .ToListAsync();
                if (requests.Count != 0)
                {
                    _context.FriendRequests.RemoveRange(requests);
                }
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> SendRequest(int senderId, int receriverId)
        {
            var request = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receriverId,
                Status = FriendshipStatus.Pending,
            };
            await _context.AddAsync(request);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<FriendRequest> UpdateRequest(int requestId, string status)
        {
            var requestDb = await _context.FriendRequests.FindAsync(requestId);
            if (requestDb != null)
            {
                requestDb.Status = status;
                requestDb.DateUpdated = DateTime.UtcNow;
                _context.Update(requestDb);

                if (status == FriendshipStatus.Accepted)
                {
                    var friendship = new Friendship
                    {
                        SenderId = requestDb.SenderId,
                        ReceiverId = requestDb.ReceiverId
                    };
                    await _context.Friendships.AddAsync(friendship);

                }
                await _context.SaveChangesAsync();

            }
            return requestDb;

        }
        
        
    }
}