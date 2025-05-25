using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircleApp.Models
{
    public class Like
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }

        //navigation properties
        public User User { get; set; }
        public Post Post { get; set; }
    }
}