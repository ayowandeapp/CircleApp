using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CircleApp.Models.Base;

namespace CircleApp.Models
{
    public class Post: SoftDeletableEntity
    {
        [Key]
        public int Id { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public int NrOfReports { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool IsPrivate { get; set; }
        //Foreign Key
        public int UserId { get; set; }
        //Navigation Properties
        public User User { get; set; }

        public ICollection<Like> Likes { get; set; } = [];
        public ICollection<Comment> Comments { get; set; } = [];
        public ICollection<Favorite> Favorites { get; set; } = [];
        public ICollection<Report> Reports { get; set; } = [];
    }
}