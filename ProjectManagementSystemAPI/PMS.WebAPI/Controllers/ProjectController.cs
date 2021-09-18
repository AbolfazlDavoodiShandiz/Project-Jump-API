using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.Common.Enums;
using PMS.Common.Utility;
using PMS.DTO;
using PMS.Entities;
using PMS.Services;
using PMS.WebFramework.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;

        public ProjectController(IProjectService projectService, IMapper mapper)
        {
            _projectService = projectService;
            _mapper = mapper;
        }

        [HttpGet]
        [ActionName("GetAllUserProjects")]
        public async Task<ApiResult<IEnumerable<ProjectDTO>>> GetAllUserProjects(CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();

            var list = await _projectService.GetAllByUserId(userId, cancellationToken);

            if (list is not null && list.Count() > 0)
            {
                var mappedList = _mapper.Map<IEnumerable<ProjectDTO>>(list);
                return Ok(mappedList);
            }
            else
            {
                throw new AppException(HttpStatusCode.NotFound, "There is no project(s) for this user.");
            }
        }

        [HttpPost]
        [ActionName("CreateProject")]
        public async Task<ApiResult> CreateProject(ProjectRegistrationDTO projectRegistrationDTO, CancellationToken cancellationToken)
        {
            if (await _projectService.ExistsByTitle(projectRegistrationDTO.Title, cancellationToken))
            {
                throw new AppException(HttpStatusCode.BadRequest, "This project title is not available.");
            }

            var userId = User.Identity.GetUserId();

            var project = _mapper.Map<Project>(projectRegistrationDTO);
            project.OwnerId = userId;

            await _projectService.CreateProject(project, cancellationToken);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "Project created successfully.");
        }

        [HttpPost]
        [ActionName("EditProject")]
        public async Task<ApiResult> EditProject(ProjectEditDTO projectEditDTO, CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();
            var project = await _projectService.GetUserProjectById(projectEditDTO.ProjectId, userId, cancellationToken);

            if (project is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "There is no project with this data.");
            }

            project.Description = string.IsNullOrWhiteSpace(projectEditDTO.Description) ? project.Description : projectEditDTO.Description;
            project.DeadlineDate = projectEditDTO.DeadlineDate.HasValue ? projectEditDTO.DeadlineDate.Value : project.DeadlineDate;

            await _projectService.EditProject(project, cancellationToken);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "Project updated successfully.");
        }

        [HttpPost]
        [ActionName("DeleteProject")]
        public async Task<ApiResult> DeleteProject(ProjectDeleteDTO projectDeleteDTO, CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();
            var project = await _projectService.GetUserProjectById(projectDeleteDTO.ProjectId, userId, cancellationToken);

            if (project is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "There is no project with this data.");
            }

            await _projectService.DeleteProject(project, cancellationToken);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "Project deleted successfully.");
        }
    }
}
