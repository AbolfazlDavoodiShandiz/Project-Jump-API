using Microsoft.EntityFrameworkCore;
using PMS.Common.Enums;
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
    public class ProjectTaskService : IProjectTaskService
    {
        private readonly IRepository<ProjectTask> _projectTaskRepository;
        private readonly IRepository<UserTask> _userTaskRepository;
        private readonly INotificationService _notificationService;

        public ProjectTaskService(IRepository<ProjectTask> projectTaskRepository, IRepository<UserTask> userTaskRepository, INotificationService notificationService)
        {
            _projectTaskRepository = projectTaskRepository;
            _userTaskRepository = userTaskRepository;
            _notificationService = notificationService;
        }

        public async Task AssignProjectTaskToProjectMember(int taskId, int userId, int registerUserId, CancellationToken cancellationToken)
        {
            var exist = await _userTaskRepository
                .TableNoTracking
                .Where(ut => ut.UserId == userId && ut.TaskId == taskId)
                .AnyAsync(cancellationToken);

            if (!exist)
            {
                var userTask = new UserTask()
                {
                    TaskId = taskId,
                    UserId = userId,
                    RegisterUserId = registerUserId
                };

                await _userTaskRepository.AddAsync(userTask, cancellationToken);

                var task = await _projectTaskRepository.GetByIdAsync(cancellationToken, userTask.TaskId);

                await _notificationService.Create(userId, NotificationType.AssignTask, task.Title, task.Id, cancellationToken);
            }
        }

        public async Task CreateProjectTask(ProjectTask projectTask, CancellationToken cancellationToken)
        {
            await _projectTaskRepository.AddAsync(projectTask, cancellationToken);
        }

        public async Task DeleteTask(ProjectTask projectTask, CancellationToken cancellationToken)
        {
            await _projectTaskRepository.DeleteAsync(projectTask, cancellationToken);
        }

        public async Task EditProjectTask(ProjectTask projectTask, CancellationToken cancellationToken)
        {
            await _projectTaskRepository.UpdateAsync(projectTask, cancellationToken);
        }

        public async Task<IEnumerable<ProjectTask>> GetAllByUserId(int userId, CancellationToken cancellationToken,
            bool justIncompletedTasks = true, bool justCompletedTask = true)
        {
            IEnumerable<ProjectTask> tasks = null;

            if (justCompletedTask && !justIncompletedTasks)
            {
                tasks = await _userTaskRepository.TableNoTracking
                    .Where(ut => ut.UserId == userId && ut.ProjectTask.Done)
                    .Include(ut => ut.ProjectTask)
                    .Include(ut => ut.ProjectTask.Owner)
                    .Include(ut => ut.ProjectTask.Project)
                    //.Where(ut => ut.ProjectTask.Done)
                    .Select(ut => ut.ProjectTask)
                    .ToListAsync(cancellationToken);
            }
            else if (justIncompletedTasks && !justCompletedTask)
            {
                tasks = await _userTaskRepository.TableNoTracking
                .Where(ut => ut.UserId == userId && !ut.ProjectTask.Done)
                .Include(ut => ut.ProjectTask)
                .Include(ut => ut.ProjectTask.Owner)
                .Include(ut => ut.ProjectTask.Project)
                //.Where(ut => !ut.ProjectTask.Done)
                .Select(ut => ut.ProjectTask)
                .ToListAsync(cancellationToken);
            }
            else if (justCompletedTask && justIncompletedTasks)
            {
                tasks = await _userTaskRepository.TableNoTracking
                .Where(ut => ut.UserId == userId)
                .Include(ut => ut.ProjectTask)
                .Include(ut => ut.ProjectTask.Owner)
                .Include(ut => ut.ProjectTask.Project)
                .Select(ut => ut.ProjectTask)
                .ToListAsync(cancellationToken);
            }

            return tasks;
        }

        public async Task<IEnumerable<ProjectTask>> GetUserCreatedTasks(int userId, CancellationToken cancellationToken)
        {
            var tasks = await _projectTaskRepository
                .TableNoTracking
                .Where(t => t.OwnerId == userId)
                .Include(t => t.UserTasks)
                .ToListAsync(cancellationToken);

            return tasks;
        }

        public async Task<ProjectTask> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _projectTaskRepository
                .TableNoTracking
                .Where(t => t.Id == id)
                .Include(t => t.Project)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task MarkAsDone(int taskId, CancellationToken cancellationToken)
        {
            var task = await _projectTaskRepository
                .TableNoTracking
                .Where(t => t.Id == taskId)
                .SingleAsync(cancellationToken);

            task.Done = true;
            task.CompleteDate = DateTime.Now;

            await _projectTaskRepository.UpdateAsync(task, cancellationToken);
        }

        public async Task<bool> IsAssigned(int userId, int taskId, CancellationToken cancellationToken)
        {
            return await _userTaskRepository
                .TableNoTracking
                .AnyAsync(t => t.TaskId == taskId && t.UserId == userId, cancellationToken);
        }

        public async Task DeleteAssignedProjectTask(int taskId, int userId, CancellationToken cancellationToken)
        {
            var userTask = await _userTaskRepository.TableNoTracking.Where(ut => ut.TaskId == taskId && ut.UserId == userId).SingleOrDefaultAsync(cancellationToken);

            if (userTask is not null)
            {
                await _userTaskRepository.DeleteAsync(userTask, cancellationToken);
            }
        }

        public async Task DeleteAssignedProjectTaskInRange(IEnumerable<UserTask> userTasks, CancellationToken cancellationToken)
        {
            await _userTaskRepository.DeleteRangeAsync(userTasks, cancellationToken);
        }

        public async Task<IEnumerable<ProjectTask>> GetAllByProjectId(int projectId, CancellationToken cancellationToken)
        {
            return await _projectTaskRepository
                .TableNoTracking
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.UserTasks)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<string>> GetTaskAssignedUser(int taskId, CancellationToken cancellationToken)
        {
            var userTask = await _userTaskRepository
                .TableNoTracking
                .Where(ut => ut.TaskId == taskId)
                .Include(ut => ut.User)
                .Select(ut => ut.User.UserName)
                .ToListAsync();

            return userTask;
        }
    }
}
