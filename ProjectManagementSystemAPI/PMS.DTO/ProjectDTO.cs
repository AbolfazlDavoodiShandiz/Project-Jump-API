using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.DTO
{
    public class ProjectDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; internal set; }
        public DateTime DeadlineDate { get; set; }
    }
}
