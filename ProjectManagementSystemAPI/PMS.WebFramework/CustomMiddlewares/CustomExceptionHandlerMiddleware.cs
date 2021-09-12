using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PMS.Common.Enums;
using PMS.Common.Utility;
using PMS.WebFramework.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PMS.WebFramework.CustomMiddlewares
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            _next = next;
            _env = env;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (AppException appEx)
            {
                var apiResult = new ApiResult(false, ApiResponseStatus.Failure, appEx.HttpStatusCode, appEx.Message);
                var result = JsonConvert.SerializeObject(apiResult);

                await httpContext.Response.WriteAsync(result);
            }
            catch (Exception ex)
            {
                var apiResult = new ApiResult(false, ApiResponseStatus.Failure, HttpStatusCode.InternalServerError, "An unhandeled server error has occured.Please contact with api support specialist.");
                var result = JsonConvert.SerializeObject(apiResult);

                await httpContext.Response.WriteAsync(result);
            }
        }
    }
}
