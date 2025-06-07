using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Models;

namespace CircleApp.ViewModels.Users
{
    public class GetUserProfileVM
    {
        public List<Post> Posts { get; set; }
        public User User { get; set; }
    }
}