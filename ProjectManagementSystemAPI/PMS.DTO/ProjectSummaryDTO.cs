using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.DTO
{
    public class ProjectSummaryDTO
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public DateTime ProjectDeadlineDate { get; set; }
        public IEnumerable<ProjectTaskDTO> ProjectTasks { get; set; }
        public IEnumerable<ProjectMemberDTO> ProjectMembers { get; set; }
    }
}
