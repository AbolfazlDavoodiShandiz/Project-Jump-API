using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.DTO
{
    public class ProjectTaskRegistrationDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DeadlineDate { get; set; }
        public int ProjectId { get; set; }
        public bool Done { get; set; }
    }
}
