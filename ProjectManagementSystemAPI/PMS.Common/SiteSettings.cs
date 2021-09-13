﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Common
{
    public class SiteSettings
    {
        public IdentitySettings IdentitySettings { get; set; }
        public JwtSettings JwtSettings { get; set; }
    }

    public class IdentitySettings
    {
        public bool PasswordRequireDigit { get; set; }
        public int PasswordRequiredLength { get; set; }
        public bool PasswordRequireLowercase { get; set; }
        public bool PasswordRequireUppercase { get; set; }
        public bool PasswordRequireNonAlphanumeric { get; set; }
        public int PasswordRequiredUniqueChars { get; set; }
        public bool UserRequireUniqueEmail { get; set; }
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string EncryptKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpirationMinutes { get; set; }
    }
}