using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using PMS.Common;
using PMS.Common.Enums;
using PMS.Common.Utility;
using PMS.DTO;
using PMS.Entities;
using PMS.Services;
using PMS.WebFramework.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.WebAPI.Controllers
{
    [Route("api/account/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;
        private readonly SiteSettings _siteSettings;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager,
            IJwtService jwtServices, IOptionsSnapshot<SiteSettings> siteSettings, IMapper mapper,
            IDistributedCache cache)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtService = jwtServices;
            _siteSettings = siteSettings.Value;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpPost]
        [ActionName("UserRegister")]
        [AllowAnonymous]
        public async Task<ApiResult> UserRegister(UserRegistrationDTO userRegistrationDTO)
        {
            var user = new User
            {
                Email = userRegistrationDTO.Email,
                UserName = userRegistrationDTO.Username,
                PhoneNumber = userRegistrationDTO.Mobile
            };

            var result = await _userManager.CreateAsync(user, userRegistrationDTO.Password);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "User registered successfully.");
        }

        [HttpPost]
        [ActionName("UserLogin")]
        [AllowAnonymous]
        public async Task<ApiResult<UserLoginResponseDTO>> UserLogin(UserLoginRequestDTO userLoginRequestDTO)
        {
            var user = await _userManager.FindByEmailAsync(userLoginRequestDTO.Email);

            if (user is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "User not exists.");
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, userLoginRequestDTO.Password, false);

            if (signInResult != Microsoft.AspNetCore.Identity.SignInResult.Success)
            {
                throw new AppException(HttpStatusCode.BadRequest, "Username or password is incorrect.");
            }

            var token = await _jwtService.GenerateAsync(user);
            var tokenExpireDate = DateTime.Now.AddMinutes(_siteSettings.JwtSettings.ExpirationMinutes);
            var userClaimPrincipal = await _signInManager.ClaimsFactory.CreateAsync(user);
            var userClaims = new Dictionary<string, object>();

            foreach (var claim in userClaimPrincipal.Claims)
            {
                userClaims.Add(claim.Type, claim.Value);
            }

            user.LastLoginDate = DateTime.Now;
            user.LastCreatedToken = token;
            user.LastTokenExpireDate = tokenExpireDate;

            await _userManager.UpdateAsync(user);

            var loginResponseDTO = new UserLoginResponseDTO
            {
                Username = user.UserName,
                DisplayName = user.DisplayName,
                Email = user.Email,
                UserClaims = userClaims,
                Token = token,
                TokenExpirationDate = tokenExpireDate
            };

            return new ApiResult<UserLoginResponseDTO>(true, ApiResponseStatus.Success, HttpStatusCode.OK, loginResponseDTO);
        }

        [HttpPost]
        [ActionName("UserLogout")]
        public async Task<ApiResult> UserLogout(UserEmailDTO userEmailDTO)
        {
            var user = await _userManager.FindByEmailAsync(userEmailDTO.Email);

            if (user is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "User not found.");
            }

            user.LastCreatedToken = null;
            user.LastTokenExpireDate = null;

            await _userManager.UpdateAsync(user);

            string userCacheKey = $"UserId_{user.Id}";
            await _cache.RemoveAsync(userCacheKey);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "User logged out successfully.");
        }

        [HttpGet("{email}")]
        [ActionName("SearchUserByEmail")]
        public async Task<ApiResult<IEnumerable<UserSearchResponseDTO>>> SearchUserByEmail(string email, CancellationToken cancellationToken)
        {
            var users = await _userManager.Users.Where(u => u.Email.Contains(email)).ToListAsync(cancellationToken);

            var result = _mapper.Map<IEnumerable<UserSearchResponseDTO>>(users);

            return Ok(result);
        }
    }
}
