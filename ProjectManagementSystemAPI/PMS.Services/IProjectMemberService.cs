using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.Services
{
    public interface IProjectMemberService
    {
        Task AddProjectMember(ProjectMember projectMember, CancellationToken cancellationToken);
        Task AddProjectMember(List<ProjectMember> projectMembers, CancellationToken cancellationToken);
        Task DeleteProjectMember(ProjectMember projectMember, CancellationToken cancellationToken);
        Task DeleteProjectMemberInRange(IEnumerable<ProjectMember> projectMembers, CancellationToken cancellationToken);
        Task<bool> IsProjectMember(int userId, int projectId, CancellationToken cancellationToken);
        Task<IEnumerable<ProjectMember>> GetProjectMembers(int projectId, CancellationToken cancellationToken);
    }
}
