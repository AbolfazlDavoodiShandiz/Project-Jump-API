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
    }
}
