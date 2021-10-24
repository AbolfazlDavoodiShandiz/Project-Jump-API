using Newtonsoft.Json;
using PMS.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Common.Utility
{
    public class AppException : Exception
    {
        public AppException(HttpStatusCode httpStatusCode, string message, object additionalData = null)
            : base(message)
        {
            HttpStatusCode = httpStatusCode;
            AdditionalData = additionalData;
        }

        public HttpStatusCode HttpStatusCode { get; }
        public int HttpStatusCodeNumber
        {
            get
            {
                return (int)HttpStatusCode;
            }
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object AdditionalData { get; }
    }
}
