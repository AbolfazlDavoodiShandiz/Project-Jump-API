using Microsoft.AspNetCore.Identity;
using PMS.Common.Enums;
using PMS.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PMS.WebFramework.Configurations
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateEmail(string email)
        {
            throw new AppException( HttpStatusCode.BadRequest, "There is an user with this email.");
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            throw new AppException( HttpStatusCode.BadRequest, "There is an user with this username.");
        }

        public override IdentityError InvalidEmail(string email)
        {
            throw new AppException( HttpStatusCode.BadRequest, "Email is invalid.");
        }

        public override IdentityError InvalidToken()
        {
            throw new AppException( HttpStatusCode.BadRequest, "Security token is invalid.");
        }

        public override IdentityError InvalidUserName(string userName)
        {
            throw new AppException( HttpStatusCode.BadRequest, "Username is invalid.");
        }

        public override IdentityError PasswordMismatch()
        {
            throw new AppException( HttpStatusCode.BadRequest, "Password and it's confirmation are different.");
        }

        public override IdentityError PasswordRequiresDigit()
        {
            throw new AppException( HttpStatusCode.BadRequest, "Password must have digits.");
        }

        public override IdentityError PasswordRequiresLower()
        {
            throw new AppException( HttpStatusCode.BadRequest, "Password must have lowercase characters.");
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            throw new AppException( HttpStatusCode.BadRequest, "Password must have alphanameric characters.");
        }

        public override IdentityError PasswordRequiresUpper()
        {
            throw new AppException( HttpStatusCode.BadRequest, "Password must have uppercase characters.");
        }

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
        {
            throw new AppException( HttpStatusCode.BadRequest, "Password must have unique characters.");
        }

        public override IdentityError PasswordTooShort(int length)
        {
            throw new AppException( HttpStatusCode.BadRequest, "Password is too short.");
        }
    }
}
