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
    [Route("api/projecttask/[action]")]
    [ApiController]
    public class ProjectTaskController : ControllerBase
    {
        private readonly IProjectTaskService _projectTaskService;
        private readonly IProjectMemberService _projectMemberService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public ProjectTaskController(IProjectTaskService projectTaskService, IProjectMemberService projectMemberService, UserManager<User> userManager, IMapper mapper)
        {
            _projectTaskService = projectTaskService;
            _projectMemberService = projectMemberService;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpPost]
        [ActionName("CreateProjectTask")]
        public async Task<ApiResult> CreateProjectTask(ProjectTaskRegistrationDTO projectTaskRegistrationDTO, CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();

            var projectTask = _mapper.Map<ProjectTask>(projectTaskRegistrationDTO);
            projectTask.OwnerId = userId;

            await _projectTaskService.CreateProjectTask(projectTask, cancellationToken);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "Project task created successfully.");
        }

        [HttpPost]
        [ActionName("EditProjectTask")]
        public async Task<ApiResult> EditProjectTask(ProjectTaskEditDTO projectTaskEditDTO, CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();
            var task = await _projectTaskService.GetByIdAsync(projectTaskEditDTO.TaskId, cancellationToken);

            if (task is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "There is no task with this data.");
            }

            if (task.OwnerId != userId)
            {
                throw new AppException(HttpStatusCode.BadRequest, "You are not authorized to perform this action.");
            }

            task.Description = projectTaskEditDTO.Description;
            task.DeadlineDate = projectTaskEditDTO.DeadlineDate;

            await _projectTaskService.EditProjectTask(task, cancellationToken);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "Task updated successfully.");
        }

        [HttpPost]
        [ActionName("DeleteProjectTask")]
        public async Task<ApiResult> DeleteProjectTask(EntityIdDTO taskId, CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();
            var task = await _projectTaskService.GetByIdAsync(taskId.Id, cancellationToken);

            if (task.OwnerId != userId)
            {
                throw new AppException(HttpStatusCode.BadRequest, "You are not authorized to perform this action.");
            }

            await _projectTaskService.DeleteTask(task, cancellationToken);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "Task deleted successfully");
        }

        [HttpGet]
        [ActionName("UserCreatedTaskList")]
        public async Task<ApiResult<IEnumerable<ProjectTaskDTO>>> UserCreatedTaskList(CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();

            var tasks = await _projectTaskService.GetUserCreatedTasks(userId, cancellationToken);

            if (tasks is not null && tasks.Count() > 0)
            {
                var mapped = _mapper.Map<IEnumerable<ProjectTaskDTO>>(tasks);

                return Ok(mapped);
            }
            else
            {
                throw new AppException(HttpStatusCode.NotFound, "There is no task(s) created by this user.");
            }
        }

        [HttpGet]
        [ActionName("UserAssignedTaskList")]
        public async Task<ApiResult<IEnumerable<ProjectTaskDTO>>> UserAssignedTaskList(CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();

            var tasks = await _projectTaskService.GetAllByUserId(userId, cancellationToken);

            if (tasks is not null && tasks.Count() > 0)
            {
                var mapped = _mapper.Map<IEnumerable<ProjectTaskDTO>>(tasks);
                return Ok(mapped);
            }
            else
            {
                throw new AppException(HttpStatusCode.NotFound, "There is no task(s) for this user.");
            }
        }

        [HttpGet]
        [ActionName("UserCompletedTaskList")]
        public async Task<ApiResult<IEnumerable<ProjectTaskDTO>>> UserCompletedTaskList(CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();

            var tasks = await _projectTaskService.GetAllByUserId(userId, cancellationToken, false, true);

            if (tasks is not null && tasks.Count() > 0)
            {
                var mapped = _mapper.Map<IEnumerable<ProjectTaskDTO>>(tasks);
                return Ok(mapped);
            }
            else
            {
                throw new AppException(HttpStatusCode.NotFound, "There is no completed task(s) for this user.");
            }
        }

        [HttpGet]
        [ActionName("UserIncompleteTaskList")]
        public async Task<ApiResult<IEnumerable<ProjectTaskDTO>>> UserIncompleteTaskList(CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();

            var tasks = await _projectTaskService.GetAllByUserId(userId, cancellationToken, true, false);

            if (tasks is not null && tasks.Count() > 0)
            {
                var mapped = _mapper.Map<IEnumerable<ProjectTaskDTO>>(tasks);
                return Ok(mapped);
            }
            else
            {
                throw new AppException(HttpStatusCode.NotFound, "There is no incompleted task(s) for this user.");
            }
        }

        [HttpPost]
        [ActionName("AssignTaskToProjectMember")]
        public async Task<ApiResult> AssignTaskToProjectMember(ProjectTaskAssignToMemberDTO projectTaskAssignToMemberDTO, CancellationToken cancellationToken)
        {
            var currentUserId = User.Identity.GetUserId();

            var task = await _projectTaskService.GetByIdAsync(projectTaskAssignToMemberDTO.TaskId, cancellationToken);

            if (task is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "This task doesn't exist.");
            }

            if (task.OwnerId != currentUserId)
            {
                throw new AppException(HttpStatusCode.BadRequest, "You don't have permission to assign this task.");
            }

            var user = await _userManager.FindByIdAsync(projectTaskAssignToMemberDTO.UserId.ToString());

            if (user is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "This user doesn't exist.");
            }

            var isProjectMember = await _projectMemberService.IsProjectMember(user.Id, task.Project.Id, cancellationToken);

            if (!isProjectMember)
            {
                throw new AppException(HttpStatusCode.BadRequest, "User is not a member of task's project team.");
            }

            await _projectTaskService.AssignProjectTaskToProjectMember(projectTaskAssignToMemberDTO.TaskId, projectTaskAssignToMemberDTO.UserId, currentUserId, cancellationToken);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "Task assigned to user successfully.");
        }

        [HttpPost]
        [ActionName("UnassignTask")]
        public async Task<ApiResult> DeleteAssignedUserTask(ProjectTaskAssignToMemberDTO projectTaskAssignToMemberDTO, CancellationToken cancellationToken)
        {
            var currentUserId = User.Identity.GetUserId();

            var task = await _projectTaskService.GetByIdAsync(projectTaskAssignToMemberDTO.TaskId, cancellationToken);

            if (task is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "This task doesn't exist.");
            }

            if (task.OwnerId != currentUserId)
            {
                throw new AppException(HttpStatusCode.BadRequest, "You don't have permission to unassign this task.");
            }

            var user = await _userManager.FindByIdAsync(projectTaskAssignToMemberDTO.UserId.ToString());

            if (user is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "This user doesn't exist.");
            }

            var isAssigned = await _projectTaskService.IsAssigned(projectTaskAssignToMemberDTO.UserId, projectTaskAssignToMemberDTO.TaskId, cancellationToken);

            if (!isAssigned)
            {
                throw new AppException(HttpStatusCode.BadRequest, "This task didn't assign to this user");
            }

            await _projectTaskService.DeleteAssignedProjectTask(projectTaskAssignToMemberDTO.TaskId, projectTaskAssignToMemberDTO.UserId, cancellationToken);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "Task unassigned successfully.");
        }

        [HttpPost]
        [ActionName("MarkTaskAsDone")]
        public async Task<ApiResult> MarkTaskAsDone(EntityIdDTO TaskDoneDTO, CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();
            var task = await _projectTaskService.GetByIdAsync(TaskDoneDTO.Id, cancellationToken);

            if (task.OwnerId != userId)
            {
                var isAssigned = await _projectTaskService.IsAssigned(userId, TaskDoneDTO.Id, cancellationToken);

                if (!isAssigned)
                {
                    throw new AppException(HttpStatusCode.NotFound, "Task id or user id is incorrect or this task isn't assigned to this user.");
                }
            }

            await _projectTaskService.MarkAsDone(TaskDoneDTO.Id, cancellationToken);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "Task marked as done.");
        }
    }
}
