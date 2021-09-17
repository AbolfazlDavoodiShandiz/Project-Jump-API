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

        public ProjectService(IRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task CreateProject(Project project, CancellationToken cancellationToken)
        {
            await _projectRepository.AddAsync(project, cancellationToken);
        }

        public async Task DeleteProject(Project project, CancellationToken cancellationToken)
        {
            await _projectRepository.DeleteAsync(project, cancellationToken);
        }

        public async Task EditProject(Project project, CancellationToken cancellationToken)
        {
            await _projectRepository.UpdateAsync(project, cancellationToken);
        }

        public async Task<bool> ExistsByTitle(string name, CancellationToken cancellationToken)
        {
            var exists = await _projectRepository.TableNoTracking.AnyAsync(p => p.Title == name, cancellationToken);

            return exists;
        }

        public async Task<IEnumerable<Project>> GetAll(int userId, CancellationToken cancellationToken)
        {
            return await _projectRepository.TableNoTracking.Where(p => p.OwnerId == userId).Include(p => p.Tasks).ToListAsync(cancellationToken);
        }

        public async Task<Project> GetUserProjectById(int projectId, int userId, CancellationToken cancellationToken)
        {
            return await _projectRepository.TableNoTracking.Where(p => p.Id == projectId && p.OwnerId == userId).SingleOrDefaultAsync(cancellationToken);
        }
    }
}
