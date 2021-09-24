using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PMS.Common;
using PMS.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PMS.Services.Implementations
{
    public class JwtService : IJwtService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly SiteSettings _siteSettings;

        public JwtService(SignInManager<User> signInManager, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _signInManager = signInManager;
            _siteSettings = siteSettings.Value;
        }

        public async Task<string> GenerateAsync(User user)
        {
            string token = string.Empty;

            var secretKey = Encoding.UTF8.GetBytes(_siteSettings.JwtSettings.SecretKey);
            var encryptKey = Encoding.UTF8.GetBytes(_siteSettings.JwtSettings.EncryptKey);

            var signingCredential = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256);
            var encriptingCredential = new EncryptingCredentials(new SymmetricSecurityKey(encryptKey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            IEnumerable<Claim> claims = await GetClaims(user);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _siteSettings.JwtSettings.Issuer,
                IssuedAt = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(_siteSettings.JwtSettings.ExpirationMinutes),
                Audience = _siteSettings.JwtSettings.Audience,
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = signingCredential,
                EncryptingCredentials = encriptingCredential
            };

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            token = tokenHandler.WriteToken(securityToken);

            return token;
        }

        private async Task<IEnumerable<Claim>> GetClaims(User user)
        {
            var result = await _signInManager.ClaimsFactory.CreateAsync(user);
            return new List<Claim>(result.Claims);
        }
    }
}
