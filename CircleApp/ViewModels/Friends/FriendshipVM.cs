using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Models;

namespace CircleApp.ViewModels.Friends
{
    public class FriendshipVM
    {
        public List<FriendRequest> FriendRequestsSent = [];
        public List<FriendRequest> FriendRequestsReceived = [];
        public List<Friendship> Friends = [];
    }
}