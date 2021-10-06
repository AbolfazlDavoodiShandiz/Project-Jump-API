using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.Services.Implementations
{
    public class ProjectMemberService : IProjectMemberService
    {
        private readonly IRepository<ProjectMember> _projectMemberRepository;

        public ProjectMemberService(IRepository<ProjectMember> projectMemberRepository)
        {
            _projectMemberRepository = projectMemberRepository;
        }

        public async Task AddProjectMember(ProjectMember projectMember, CancellationToken cancellationToken)
        {
            await _projectMemberRepository.AddAsync(projectMember, cancellationToken);
        }

        public async Task DeleteProjectMember(ProjectMember projectMember, CancellationToken cancellationToken)
        {
            await _projectMemberRepository.DeleteAsync(projectMember, cancellationToken);
        }

        public async Task DeleteProjectMemberInRange(IEnumerable<ProjectMember> projectMembers, CancellationToken cancellationToken)
        {
            await _projectMemberRepository.DeleteRangeAsync(projectMembers, cancellationToken);
        }

        public async Task<bool> IsProjectMember(int userId, int projectId, CancellationToken cancellationToken)
        {
            return await _projectMemberRepository.TableNoTracking.AnyAsync(m => m.ProjectId == projectId && m.UserId == userId, cancellationToken);
        }
    }
}
