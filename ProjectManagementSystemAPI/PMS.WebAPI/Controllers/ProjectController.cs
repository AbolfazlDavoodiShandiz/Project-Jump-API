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
        [ActionName("GetAll")]
        public async Task<ApiResult<IEnumerable<ProjectDTO>>> GetAll(CancellationToken cancellationToken)
        {
            var userId = User.Identity.GetUserId();

            var list = await _projectService.GetAll(userId, cancellationToken);

            if (list is not null && list.Count() > 0)
            {
                var mappedList = _mapper.Map<IEnumerable<ProjectDTO>>(list);
                return Ok(mappedList);
            }
            else
            {
                return NotFound();
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

            var result = await _projectService.CreateProject(project, cancellationToken);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "Project created successfully.");
        }
    }
}
