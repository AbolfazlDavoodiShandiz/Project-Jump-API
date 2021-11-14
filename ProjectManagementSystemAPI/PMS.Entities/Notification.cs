using PMS.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Entities
{
    public class Notification : BaseEntity
    {
        public Notification()
        {
            IsRead = false;
            CreateDate = DateTime.Now;
        }

        public int CreatedUserId { get; set; }
        public string CreatedUsername { get; set; }
        public int RecieverUserId { get; set; }
        public User RecieverUser { get; set; }
        public NotificationType NotificationType { get; set; }
        public string RelatedObjectTitle { get; set; }
        public int RelatedObjectId { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsRead { get; set; }
    }
}
