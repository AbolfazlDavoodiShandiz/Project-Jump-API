﻿using PMS.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> GetAll(int userId, CancellationToken cancellationToken);
        Task<Project> GetUserProjectById(int projectId, int userId, CancellationToken cancellationToken);
        Task CreateProject(Project project, CancellationToken cancellationToken);
        Task<bool> ExistsByTitle(string name, CancellationToken cancellationToken);
        Task EditProject(Project project, CancellationToken cancellationToken);
        Task DeleteProject(Project project, CancellationToken cancellationToken);
    }
}