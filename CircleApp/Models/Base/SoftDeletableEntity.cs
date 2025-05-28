using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircleApp.Models.Base
{
    public abstract class SoftDeletableEntity
    {
        public DateTime? DateDeleted { get; protected set; } //

        public bool IsDeleted => DateDeleted != null;

        public void SoftDelete()
        {
            DateDeleted = DateTime.UtcNow;
        }
        
        public void Restore() 
        {
            DateDeleted = null;
        }
    }
}