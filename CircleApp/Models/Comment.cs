using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CircleApp.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime DateUpdated { get; set; }
        //Foreign Key
        public int UserId { get; set; }
        public int PostId { get; set; }
        //Navigation properties
        public User User { get; set; }
        public Post Post { get; set; }

        

    }
}