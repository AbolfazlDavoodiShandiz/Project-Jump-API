using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMS.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PMS.WebFramework.API
{
    public class ApiResult
    {
        public ApiResult(bool isSuccess, ApiResponseStatus apiResponseStatus, HttpStatusCode httpStatusCode, string message = null)
        {
            IsSuccess = isSuccess;
            ApiResponseStatus = apiResponseStatus;
            HttpStatusCode = httpStatusCode;
            Message = message;
        }

        public bool IsSuccess { get; }
        public ApiResponseStatus ApiResponseStatus { get; }
        public HttpStatusCode HttpStatusCode { get; }
        public string ApiResponseStatusName
        {
            get
            {
                return ApiResponseStatus.ToString();
            }
        }
        public int HttpStatusCodeNumber
        {
            get
            {
                return (int)HttpStatusCode;
            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        public static implicit operator ApiResult(OkResult okResult)
        {
            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK);
        }

        public static implicit operator ApiResult(BadRequestResult badRequestResult)
        {
            return new ApiResult(false, ApiResponseStatus.Failure, HttpStatusCode.BadRequest);
        }

        public static implicit operator ApiResult(BadRequestObjectResult badRequestObjectResult)
        {
            string message = badRequestObjectResult.Value?.ToString();

            if (badRequestObjectResult.Value is SerializableError errors)
            {
                var errorMessagses = errors.SelectMany(e => (string[])e.Value).Distinct();
                message = string.Join(" | ", errorMessagses);
            }
            else if (badRequestObjectResult.Value is ValidationProblemDetails problems)
            {
                var errorMessagses = problems.Errors.SelectMany(e => e.Value).Distinct();
                message = string.Join(" | ", errorMessagses);
            }

            return new ApiResult(false, ApiResponseStatus.Failure, HttpStatusCode.BadRequest, message);
        }

        public static implicit operator ApiResult(ContentResult contentResult)
        {
            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, contentResult.Content);
        }

        public static implicit operator ApiResult(NotFoundResult notFoundResult)
        {
            return new ApiResult(false, ApiResponseStatus.Failure, HttpStatusCode.NotFound);
        }
    }

    public class ApiResult<TData> : ApiResult where TData : class
    {
        public ApiResult(bool isSuccess, ApiResponseStatus apiResponseStatus, HttpStatusCode httpStatusCode, TData data, string message = null)
            : base(isSuccess, apiResponseStatus, httpStatusCode, message)
        {
            Data = data;
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TData Data { get; }


        public static implicit operator ApiResult<TData>(OkResult okResult)
        {
            return new ApiResult<TData>(true, ApiResponseStatus.Success, HttpStatusCode.OK, null);
        }

        public static implicit operator ApiResult<TData>(OkObjectResult okObjectResult)
        {
            return new ApiResult<TData>(true, ApiResponseStatus.Success, HttpStatusCode.OK, (TData)okObjectResult.Value);
        }

        public static implicit operator ApiResult<TData>(BadRequestResult badRequestResult)
        {
            return new ApiResult<TData>(false, ApiResponseStatus.Failure, HttpStatusCode.BadRequest, null);
        }

        public static implicit operator ApiResult<TData>(BadRequestObjectResult badRequestObjectResult)
        {
            string message = badRequestObjectResult.Value?.ToString();

            if (badRequestObjectResult.Value is SerializableError errors)
            {
                var errorMessagses = errors.SelectMany(e => (string[])e.Value).Distinct();
                message = string.Join(" | ", errorMessagses);
            }
            else if (badRequestObjectResult.Value is ValidationProblemDetails problems)
            {
                var errorMessagses = problems.Errors.SelectMany(e => e.Value).Distinct();
                message = string.Join(" | ", errorMessagses);
            }

            return new ApiResult<TData>(false, ApiResponseStatus.Failure, HttpStatusCode.BadRequest, null, message);
        }

        public static implicit operator ApiResult<TData>(ContentResult contentResult)
        {
            return new ApiResult<TData>(true, ApiResponseStatus.Success, HttpStatusCode.OK, null, contentResult.Content);
        }

        public static implicit operator ApiResult<TData>(NotFoundResult notFoundResult)
        {
            return new ApiResult<TData>(false, ApiResponseStatus.Failure, HttpStatusCode.NotFound, null);
        }

        public static implicit operator ApiResult<TData>(NotFoundObjectResult notFoundObjectResult)
        {
            return new ApiResult<TData>(false, ApiResponseStatus.Failure, HttpStatusCode.NotFound, (TData)notFoundObjectResult.Value);
        }
    }
}
