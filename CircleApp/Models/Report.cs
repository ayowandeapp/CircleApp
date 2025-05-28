using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CircleApp.Models
{
    public class Report
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        //navigation properties
        public User User { get; set; }
        public Post Post { get; set; }
    }
}