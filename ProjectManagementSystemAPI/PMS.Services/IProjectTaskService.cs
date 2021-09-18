using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.Services
{
    public interface IProjectTaskService
    {
        Task<IEnumerable<ProjectTask>> GetAllByUserId(int userId, CancellationToken cancellationToken, bool justIncompletedTasks = true, bool justCompletedTask = true);
        Task CreateProjectTask(ProjectTask projectTask, CancellationToken cancellationToken);
    }
}
