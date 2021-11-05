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
        Task<IEnumerable<ProjectTask>> GetAllByProjectId(int projectId, CancellationToken cancellationToken);
        Task CreateProjectTask(ProjectTask projectTask, CancellationToken cancellationToken);
        Task EditProjectTask(ProjectTask projectTask, CancellationToken cancellationToken);
        Task DeleteTask(ProjectTask projectTask, CancellationToken cancellationToken);
        Task AssignProjectTaskToProjectMember(int taskId, int userId, int registerUserId, CancellationToken cancellationToken);
        Task DeleteAssignedProjectTask(int taskId, int userId, CancellationToken cancellationToken);
        Task DeleteAssignedProjectTaskInRange(IEnumerable<UserTask> userTasks, CancellationToken cancellationToken);
        Task MarkAsDone(int taskId, CancellationToken cancellationToken);
        Task<bool> IsAssigned(int userId, int taskId, CancellationToken cancellationToken);
        Task<IEnumerable<string>> GetTaskAssignedUser(int taskId, CancellationToken cancellationToken);
    }
}
