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
        Task<ProjectTask> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<ProjectTask>> GetUserCreatedTasks(int userId, CancellationToken cancellationToken);
        Task<IEnumerable<ProjectTask>> GetAllByUserId(int userId, CancellationToken cancellationToken, bool justIncompletedTasks = true, bool justCompletedTask = true);
        Task CreateProjectTask(ProjectTask projectTask, CancellationToken cancellationToken);
        Task EditProjectTask(ProjectTask projectTask, CancellationToken cancellationToken);
        Task DeleteTask(ProjectTask projectTask, CancellationToken cancellationToken);
        Task AssignProjectTaskToProjectMember(UserTask userTask, CancellationToken cancellationToken);
    }
}
