﻿using Microsoft.EntityFrameworkCore;
using PMS.Data;
using PMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<IEnumerable<Project>> GetAll()
        {
            return await _projectRepository.TableNoTracking.Include(p => p.Tasks).ToListAsync();
        }
    }
}