using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircleApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Fullname { get; set; }
        public string? ProfilePictureUrl { get; set; }

        //Navigation properties
        public ICollection<Post> Posts { get; set; } = [];
        public ICollection<Like> Likes { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
    }
}