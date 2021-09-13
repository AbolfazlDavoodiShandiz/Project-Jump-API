using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Common.Utility
{
    public static class IdentityExtensions
    {
        public static int GetUserId(this IIdentity principal)
        {
            var claimsIdentity = principal as ClaimsIdentity;

            if (claimsIdentity is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "No claims were not found.");
            }

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "Name identifier claim was not found.");
            }

            return int.Parse(claim.Value);
        }
    }
}
