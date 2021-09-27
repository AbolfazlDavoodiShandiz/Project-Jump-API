﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
using System.Threading.Tasks;

namespace PMS.WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;
        private readonly SiteSettings _siteSettings;

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, IJwtService jwtServices, IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtService = jwtServices;
            _siteSettings = siteSettings.Value;
        }

        [HttpPost]
        [ActionName("UserRegister")]
        public async Task<ApiResult> UserRegister(UserRegistrationDTO userRegistrationDTO)
        {
            var user = new User
            {
                Email = userRegistrationDTO.Email,
                UserName = userRegistrationDTO.Username
            };

            var result = await _userManager.CreateAsync(user, userRegistrationDTO.Password);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "User registered successfully.");
        }

        [HttpPost]
        [ActionName("UserLogin")]
        public async Task<ApiResult<LoginResponseDTO>> UserLogin(UserLoginLogoutRequestDTO userLoginRequestDTO)
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

            user.LastLoginDate = DateTime.Now;
            user.LastCreatedToken = token;
            user.LastTokenExpireDate = tokenExpireDate;

            await _userManager.UpdateAsync(user);

            var loginResponseDTO = new LoginResponseDTO
            {
                Username = user.UserName,
                Email = user.Email,
                Token = token,
                TokenExpirationDate = tokenExpireDate
            };

            return new ApiResult<LoginResponseDTO>(true, ApiResponseStatus.Success, HttpStatusCode.OK, loginResponseDTO);
        }

        [HttpPost]
        [ActionName("UserLogout")]
        public async Task<ApiResult> UserLogout(UserLoginLogoutRequestDTO userLoginLogoutRequestDTO)
        {
            var user = await _userManager.FindByEmailAsync(userLoginLogoutRequestDTO.Email);

            if(user is null)
            {
                throw new AppException(HttpStatusCode.NotFound, "User not found.");
            }

            user.LastCreatedToken = null;
            user.LastTokenExpireDate = null;

            await _userManager.UpdateAsync(user);

            return new ApiResult(true, ApiResponseStatus.Success, HttpStatusCode.OK, "User logged out successfully.");
        }
    }
}
