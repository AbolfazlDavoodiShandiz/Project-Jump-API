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
    }
}
