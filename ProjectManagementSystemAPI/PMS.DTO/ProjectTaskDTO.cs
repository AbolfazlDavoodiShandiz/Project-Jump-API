using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.DTO
{
    public class ProjectTaskDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; internal set; }
        public DateTime DeadlineDate { get; set; }
        public ProjectDTO Project { get; set; }
        public int ProjectId { get; set; }
    }
}
