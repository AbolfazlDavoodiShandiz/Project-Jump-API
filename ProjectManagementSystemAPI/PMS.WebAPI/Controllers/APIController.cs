using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.Common.Enums;
using PMS.Services;
using PMS.WebFramework.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PMS.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly IAPIService _apiService;

        public APIController(IAPIService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        [ActionName("Info")]
        [AllowAnonymous]
        public ApiResult<string> Info()
        {
            return new ApiResult<string>(true, ApiResponseStatus.Success, HttpStatusCode.OK, _apiService.APISimpleInfo());
        }
    }
}
