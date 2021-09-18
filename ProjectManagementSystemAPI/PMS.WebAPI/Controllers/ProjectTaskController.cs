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
    public class ProjectTaskController : ControllerBase
    {
        private readonly IProjectTaskService _projectTaskService;
        private readonly IMapper _mapper;

        public ProjectTaskController(IProjectTaskService projectTaskService, IMapper mapper)
        {
            _projectTaskService = projectTaskService;
            _mapper = mapper;
        }

        [HttpGet]
        [ActionName("GetAllUserTasks")]
        public async Task<ApiResult<IEnumerable<ProjectTaskDTO>>> GetAllUserTasks(CancellationToken cancellationToken)
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
        [ActionName("GetAllUserCompletedTasks")]
        public async Task<ApiResult<IEnumerable<ProjectTaskDTO>>> GetAllUserCompletedTasks(CancellationToken cancellationToken)
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
        [ActionName("GetAllUserIncompletedTasks")]
        public async Task<ApiResult<IEnumerable<ProjectTaskDTO>>> GetAllUserIncompletedTasks(CancellationToken cancellationToken)
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
        [ActionName("CreateProjectTask")]
        public async Task<ApiResult> CreateProjectTask(ProjectTaskRegistrationDTO projectTaskRegistrationDTO, CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();

            var projectTask = _mapper.Map<ProjectTask>(projectTaskRegistrationDTO);
            projectTask.OwnerId = userId;

            await _projectTaskService.CreateProjectTask(projectTask, cancellationToken);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "Project task created successfully.");
        }
    }
}
