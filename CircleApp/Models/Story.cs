using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CircleApp.Models
{
    public class Story
    {
        [Key]
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }

        //navigation properties
        public User User { get; set; }
    }
}