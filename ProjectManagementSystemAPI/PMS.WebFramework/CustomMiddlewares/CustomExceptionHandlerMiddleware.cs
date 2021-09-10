using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using PMS.Common.Enums;
using PMS.WebFramework.API;
using System;
using System.Collections.Generic;
using System.Linq;
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
            catch (Exception ex)
            {
                var apiResult = new ApiResult(false, ApiResponseStatus.ServerError);
                var result = JsonConvert.SerializeObject(apiResult);

                await httpContext.Response.WriteAsync(result);
            }
        }
    }
}
