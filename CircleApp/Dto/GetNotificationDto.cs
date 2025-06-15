using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircleApp.Dto
{
    public class GetNotificationDto
    {
        public bool Success { get; set; } = false;
        public bool SendNotification { get; set; } = false;
    }
}