﻿using Microsoft.EntityFrameworkCore;
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
    public class ProjectMemberService : IProjectMemberService
    {
        private readonly IRepository<ProjectMember> _projectMemberRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly INotificationService _notificationService;

        public ProjectMemberService(IRepository<ProjectMember> projectMemberRepository, IRepository<Project> projectRepository, INotificationService notificationService)
        {
            _projectMemberRepository = projectMemberRepository;
            _projectRepository = projectRepository;
            _notificationService = notificationService;
        }

        public async Task AddProjectMember(ProjectMember projectMember, CancellationToken cancellationToken)
        {
            await AddSingleProjectMember(projectMember, cancellationToken);
        }

        public async Task AddProjectMember(List<ProjectMember> projectMembers, CancellationToken cancellationToken)
        {
            foreach (var item in projectMembers)
            {
                await AddSingleProjectMember(item, cancellationToken);
            }
        }

        private async Task AddSingleProjectMember(ProjectMember projectMember, CancellationToken cancellationToken)
        {
            var exists = await _projectMemberRepository.TableNoTracking
                .Where(pm => pm.ProjectId == projectMember.ProjectId && pm.UserId == projectMember.UserId)
                .AnyAsync(cancellationToken);

            if (!exists)
            {
                await _projectMemberRepository.AddAsync(projectMember, cancellationToken);

                var project = await _projectRepository.GetByIdAsync(cancellationToken, projectMember.ProjectId);

                await _notificationService.CreateNotificationInDB(projectMember.UserId, NotificationType.AddProjectMember, project.Title, project.Id, cancellationToken);
            }
        }

        public async Task DeleteProjectMember(int projectId, int userId, CancellationToken cancellationToken)
        {
            var projectMember = await _projectMemberRepository.Table
                .Where(m => m.UserId == userId && m.ProjectId == projectId)
                .SingleOrDefaultAsync(cancellationToken);

            if (projectMember is not null)
            {
                await _projectMemberRepository.DeleteAsync(projectMember, cancellationToken);
            }
        }

        public async Task<IEnumerable<ProjectMember>> GetProjectMembers(int projectId, CancellationToken cancellationToken)
        {
            return await _projectMemberRepository
                .TableNoTracking
                .Where(pm => pm.ProjectId == projectId && !pm.IsProjectOwner)
                .Include(pm => pm.User)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsProjectMember(int userId, int projectId, CancellationToken cancellationToken)
        {
            return await _projectMemberRepository
                .TableNoTracking
                .AnyAsync(m => m.ProjectId == projectId && m.UserId == userId, cancellationToken);
        }
    }
}
