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
    public class ProjectService : IProjectService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<ProjectTask> _projectTaskRepository;
        private readonly IRepository<UserTask> _userTaskRepository;
        private readonly IRepository<ProjectMember> _projectMemberRepository;

        public ProjectService(IRepository<Project> projectRepository, IRepository<ProjectTask> projectTaskRepository, IRepository<UserTask> userTaskRepository, IRepository<ProjectMember> projectMemberRepository)
        {
            _projectRepository = projectRepository;
            _projectTaskRepository = projectTaskRepository;
            _userTaskRepository = userTaskRepository;
            _projectMemberRepository = projectMemberRepository;
        }

        public async Task CreateProject(Project project, CancellationToken cancellationToken)
        {
            await _projectRepository.AddAsync(project, cancellationToken);
        }

        public async Task DeleteProject(Project project, CancellationToken cancellationToken)
        {
            int projectId = project.Id;

            var projectMemberList = await _projectMemberRepository.TableNoTracking.Where(pm => pm.ProjectId == projectId).ToListAsync();
            var projectTaskList = await _projectTaskRepository.TableNoTracking.Where(pt => pt.ProjectId == projectId).ToListAsync();
            var userTaskList = await _userTaskRepository.TableNoTracking.Where(ut => projectTaskList.Contains(ut.ProjectTask)).ToListAsync();

            await _userTaskRepository.DeleteRangeAsync(userTaskList, cancellationToken);
            await _projectTaskRepository.DeleteRangeAsync(projectTaskList, cancellationToken);
            await _projectMemberRepository.DeleteRangeAsync(projectMemberList, cancellationToken);
            await _projectRepository.DeleteAsync(project, cancellationToken);
        }

        public async Task EditProject(Project project, CancellationToken cancellationToken)
        {
            await _projectRepository.UpdateAsync(project, cancellationToken);
        }

        public async Task<bool> ExistsById(int id, CancellationToken cancellationToken)
        {
            var exists = await _projectRepository.TableNoTracking.AnyAsync(p => p.Id == id, cancellationToken);

            return exists;
        }

        public async Task<bool> ExistsByTitle(string name, CancellationToken cancellationToken)
        {
            var exists = await _projectRepository.TableNoTracking.AnyAsync(p => p.Title == name, cancellationToken);

            return exists;
        }

        public async Task<Project> Get(string projectTitle, CancellationToken cancellationToken)
        {
            return await _projectRepository.TableNoTracking
                .Where(p => p.Title == projectTitle)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<Project> Get(int projectId, CancellationToken cancellationToken)
        {
            return await _projectRepository.TableNoTracking
                    .Where(p => p.Id == projectId)
                    .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<Project>> GetAllByUserId(int userId, CancellationToken cancellationToken)
        {
            return await _projectRepository
                .TableNoTracking
                .Where(p => p.OwnerId == userId)
                .Include(p => p.Tasks)
                .ToListAsync(cancellationToken);
        }

        public async Task<Project> GetUserProjectById(int projectId, int userId, CancellationToken cancellationToken)
        {
            return await _projectRepository
                .TableNoTracking
                .Where(p => p.Id == projectId && p.OwnerId == userId)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
