using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly IProjectMemberService _projectMemberService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public ProjectController(IProjectService projectService, IProjectMemberService projectMemberService, UserManager<User> userManager, IMapper mapper)
        {
            _projectService = projectService;
            _projectMemberService = projectMemberService;
            _userManager = userManager;
            _mapper = mapper;
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

            var projectMember = new ProjectMember
            {
                ProjectId = project.Id,
                UserId = userId,
                IsProjectOwner = true
            };

            await _projectMemberService.AddProjectMember(projectMember, cancellationToken);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "Project created successfully.");
        }

        [HttpGet]
        [ActionName("UserCreatedProjectList")]
        public async Task<ApiResult<IEnumerable<ProjectDTO>>> UserCreatedProjectList(CancellationToken cancellationToken)
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
        public async Task<ApiResult> DeleteProject(EntityIdDTO projectDeleteDTO, CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();
            var project = await _projectService.GetUserProjectById(projectDeleteDTO.Id, userId, cancellationToken);

            if (project is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "There is no project with this data.");
            }

            await _projectService.DeleteProject(project, cancellationToken);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "Project deleted successfully.");
        }

        [HttpPost]
        [ActionName("RegisterProjectMember")]
        public async Task<ApiResult> RegisterProjectMember(ProjectMemberAddNewDTO projectMemberAddNewDTO, CancellationToken cancellationToken)
        {
            var member = await _userManager.FindByEmailAsync(projectMemberAddNewDTO.MemberEmail);

            if (member is not null)
            {
                var projectMember = new ProjectMember
                {
                    UserId = member.Id,
                    ProjectId = projectMemberAddNewDTO.ProjectId
                };

                await _projectMemberService.AddProjectMember(projectMember, cancellationToken);

                return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "Project member added sucessfully.");
            }
            else
            {
                throw new AppException(HttpStatusCode.NotFound, "This user email doesn't exist.There is no user with this email.");
            }
        }
    }
}
