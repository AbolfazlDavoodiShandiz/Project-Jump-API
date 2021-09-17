using PMS.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAll(int userId, CancellationToken cancellationToken);
        Task<bool> CreateProject(Project project, CancellationToken cancellationToken);
        Task<bool> ExistsByTitle(string name, CancellationToken cancellationToken);
    }
}