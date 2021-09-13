using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.DTO;
using PMS.Services;
using PMS.WebFramework.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.WebAPI.Controllers
{
    [Route("api/[controller]")]
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
            var list = await _projectService.GetAll();

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
    }
}
