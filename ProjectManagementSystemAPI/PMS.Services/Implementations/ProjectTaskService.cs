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

        public async Task CreateProjectTask(ProjectTask projectTask, CancellationToken cancellationToken)
        {
            await _projectTaskRepository.AddAsync(projectTask, cancellationToken);
        }

        public Task DeleteTask(ProjectTask projectTask, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task EditProjectTask(ProjectTask projectTask, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
            var tasks = await _projectTaskRepository.TableNoTracking.Where(t => t.OwnerId == userId).ToListAsync(cancellationToken);

            return tasks;
        }
    }
}
