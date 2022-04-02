using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PMS.Common.Enums;
using PMS.Common.Utility;
using PMS.DTO;
using PMS.Entities;
using PMS.Services;
using PMS.WebFramework.API;
using PMS.WebFramework.ApplicationHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.WebAPI.Controllers
{
    [Route("api/project/[action]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IProjectMemberService _projectMemberService;
        private readonly IProjectTaskService _projectTaskService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IHubContext<ApplicationHub> _hubContext;

        public ProjectController(IProjectService projectService, IProjectMemberService projectMemberService, IProjectTaskService projectTaskService, UserManager<User> userManager,
            IMapper mapper, IHubContext<ApplicationHub> hubContext)
        {
            _projectService = projectService;
            _projectMemberService = projectMemberService;
            _projectTaskService = projectTaskService;
            _userManager = userManager;
            _mapper = mapper;
            _hubContext=hubContext;
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

            await _hubContext.Clients.All.SendAsync("notification", $"{DateTime.Now} - {User.Identity.Name} created a new project.");

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

            if (project.OwnerId != userId)
            {
                throw new AppException(HttpStatusCode.BadRequest, "You are not the owner of this project.");
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

            if (project.OwnerId != userId)
            {
                throw new AppException(HttpStatusCode.BadRequest, "You are not the owner of this project.");
            }

            await _projectService.DeleteProject(project, cancellationToken);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "Project deleted successfully.");
        }

        [HttpGet("{projectTitle}")]
        [ActionName("GetProjectSummary")]
        public async Task<ApiResult<ProjectSummaryDTO>> GetProjectSummary(string projectTitle, CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();
            var project = await _projectService.Get(projectTitle, cancellationToken);

            if (project is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "Project not found.");
            }

            if (project.OwnerId != userId)
            {
                throw new AppException(HttpStatusCode.BadRequest, "You are not the owner of this project.");
            }

            var projectTasks = await _projectTaskService.GetAllByProjectId(project.Id, cancellationToken);
            var projectMember = await _projectMemberService.GetProjectMembers(project.Id, cancellationToken);

            ProjectSummaryDTO projectSummaryDTO = new ProjectSummaryDTO();
            projectSummaryDTO.ProjectId = project.Id;
            projectSummaryDTO.ProjectTitle = project.Title;
            projectSummaryDTO.ProjectDeadlineDate = project.DeadlineDate;

            if (projectTasks is not null && projectTasks.Count() > 0)
            {
                projectSummaryDTO.ProjectTasks = _mapper.Map<IEnumerable<ProjectTaskDTO>>(projectTasks);
            }

            if (projectMember is not null && projectMember.Count() > 0)
            {
                var members = projectMember.Select(pm => pm.User).ToList();
                projectSummaryDTO.ProjectMembers = _mapper.Map<IEnumerable<ProjectMemberDTO>>(members);
            }

            return Ok(projectSummaryDTO);
        }

        [HttpGet("{projectTitle}")]
        [ActionName("GetProjectTasks")]
        public async Task<ApiResult<IEnumerable<ProjectTaskDTO>>> GetProjectTasks(string projectTitle, CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();
            var project = await _projectService.Get(projectTitle, cancellationToken);

            if (project is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "Project not found.");
            }

            if (project.OwnerId != userId)
            {
                throw new AppException(HttpStatusCode.BadRequest, "You are not the owner of this project.");
            }

            var taskList = await _projectTaskService.GetAllByProjectId(project.Id, cancellationToken);

            if (taskList is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "There is no task for this project");
            }

            var tasks = _mapper.Map<IEnumerable<ProjectTaskDTO>>(taskList);

            return Ok(tasks);
        }

        [HttpGet("{projectTitle}")]
        [ActionName("GetProjectMembers")]
        public async Task<ApiResult<IEnumerable<ProjectMemberDTO>>> GetProjectMembers(string projectTitle, CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();
            var project = await _projectService.Get(projectTitle, cancellationToken);

            if (project is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "Project not found.");
            }

            var memberList = await _projectMemberService.GetProjectMembers(project.Id, cancellationToken);

            if (memberList is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "There is no member for this project");
            }

            var members = memberList.Select(pm => pm.User).ToList();

            var membersDTO = _mapper.Map<IEnumerable<ProjectMemberDTO>>(members);

            return Ok(membersDTO);
        }

        [HttpPost]
        [ActionName("AddProjectMember")]
        public async Task<ApiResult> AddProjectMember(List<ProjectMemberRegisterDTO> projectMemberRegisterDTO, CancellationToken cancellationToken)
        {
            if (projectMemberRegisterDTO is null || projectMemberRegisterDTO.Count == 0)
            {
                throw new AppException(HttpStatusCode.BadRequest, "There is no data from client.");
            }

            var userId = User.Identity.GetUserId();
            var project = await _projectService.Get(projectMemberRegisterDTO[0].ProjectId, cancellationToken);

            if (project is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "Project not found.");
            }

            if (project.OwnerId != userId)
            {
                throw new AppException(HttpStatusCode.BadRequest, "You are not the owner of this project.");
            }

            var memberList = new List<ProjectMember>();

            projectMemberRegisterDTO.ForEach(m =>
            {
                memberList.Add(new ProjectMember { ProjectId = m.ProjectId, UserId = m.UserId });
            });

            if (memberList.Count == 1 && memberList[0].UserId == User.Identity.GetUserId())
            {
                throw new AppException(HttpStatusCode.BadRequest, "Owner of project is a project member.");
            }

            await _projectMemberService.AddProjectMember(memberList, cancellationToken);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "Project member(s) added successfully.");
        }

        [HttpPost]
        [ActionName("DeleteProjectMember")]
        public async Task<ApiResult> DeleteProjectMember(ProjectMemberDeleteDTO projectMemberDeleteDTO, CancellationToken cancellationToken)
        {
            if (projectMemberDeleteDTO is null)
            {
                throw new AppException(HttpStatusCode.BadRequest, "There is no data from client.");
            }

            var project = await _projectService.Get(projectMemberDeleteDTO.ProjectName, cancellationToken);

            if (project is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "No project found.");
            }

            var userId = User.Identity.GetUserId();

            if (project.OwnerId != userId)
            {
                throw new AppException(HttpStatusCode.BadRequest, "You are not the owner of this project.");
            }

            var user = await _userManager.FindByEmailAsync(projectMemberDeleteDTO.UserEmail);

            if (user is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "No user found.");
            }

            await _projectMemberService.DeleteProjectMember(project.Id, user.Id, cancellationToken);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "Project member deleted successfully.");
        }
    }
}
