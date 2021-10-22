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
    public class ProjectTaskService : IProjectTaskService
    {
        private readonly IRepository<ProjectTask> _projectTaskRepository;
        private readonly IRepository<UserTask> _userTaskRepository;

        public ProjectTaskService(IRepository<ProjectTask> projectTaskRepository, IRepository<UserTask> userTaskRepository)
        {
            _projectTaskRepository = projectTaskRepository;
            _userTaskRepository = userTaskRepository;
        }

        public async Task AssignProjectTaskToProjectMember(UserTask userTask, CancellationToken cancellationToken)
        {
            await _userTaskRepository.AddAsync(userTask, cancellationToken);
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
                    .Where(ut => ut.UserId == userId)
                    .Include(ut => ut.ProjectTask)
                    .Where(ut => ut.ProjectTask.Done)
                    .Select(ut => ut.ProjectTask)
                    .ToListAsync(cancellationToken);
            }
            else if (justIncompletedTasks && !justCompletedTask)
            {
                tasks = await _userTaskRepository.TableNoTracking
                .Where(ut => ut.UserId == userId)
                .Include(ut => ut.ProjectTask)
                .Where(ut => !ut.ProjectTask.Done)
                .Select(ut => ut.ProjectTask)
                .ToListAsync(cancellationToken);
            }
            else if (justCompletedTask && justIncompletedTasks)
            {
                tasks = await _userTaskRepository.TableNoTracking
                .Where(ut => ut.UserId == userId)
                .Include(ut => ut.ProjectTask)
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
                .ToListAsync(cancellationToken);

            return tasks;
        }

        public async Task<ProjectTask> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _projectTaskRepository
                .TableNoTracking
                .Where(t => t.Id == id)
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
                .AnyAsync(t => t.Id == taskId && t.UserId == userId, cancellationToken);
        }

        public async Task DeleteAssignedProjectTask(UserTask userTask, CancellationToken cancellationToken)
        {
            await _userTaskRepository.DeleteAsync(userTask, cancellationToken);
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
                .ToListAsync(cancellationToken);
        }
    }
}
