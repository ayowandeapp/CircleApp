using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CircleApp.Models
{
    public class User: IdentityUser<int>
    {
        // public int Id { get; set; }
        public string Fullname { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }

        //Navigation properties
        public ICollection<Post> Posts { get; set; } = [];
        public ICollection<Like> Likes { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
        public ICollection<Favorite> Favorites { get; set; } = [];
        public ICollection<Report> Reports { get; set; } = [];
        public ICollection<Story> Stories { get; set; } = [];
    }
}