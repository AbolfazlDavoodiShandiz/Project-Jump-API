using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Entities
{
    public class UserTask : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public ProjectTask ProjectTask { get; set; }
        public int TaskId { get; set; }

        public int RegisterUserId { get; set; }
    }
}
