using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using PMS.Common;
using PMS.Common.Utility;
using PMS.Data;
using PMS.Data.Repositories;
using PMS.Entities;
using PMS.Services;
using PMS.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PMS.WebFramework.Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SqlServer"));
            });
        }

        public static void AddApplicationDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SiteSettings>(configuration.GetSection(nameof(SiteSettings)));
            services.AddScoped<IAPIService, APIService>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IProjectTaskService, ProjectTaskService>();
            services.AddScoped<IProjectMemberService, ProjectMemberService>();
            services.AddScoped<IJwtService, JwtService>();
        }

        public static void AddApplicationController(this IServiceCollection services)
        {
            services.AddControllers(options => options.Filters.Add(new AuthorizeFilter()))
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
        }

        public static void AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            SiteSettings siteSettings = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();

            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = siteSettings.IdentitySettings.PasswordRequireDigit;
                options.Password.RequiredLength = siteSettings.IdentitySettings.PasswordRequiredLength;
                options.Password.RequireLowercase = siteSettings.IdentitySettings.PasswordRequireLowercase;
                options.Password.RequireUppercase = siteSettings.IdentitySettings.PasswordRequireUppercase;
                options.Password.RequireNonAlphanumeric = siteSettings.IdentitySettings.PasswordRequireNonAlphanumeric;
                options.Password.RequiredUniqueChars = siteSettings.IdentitySettings.PasswordRequiredUniqueChars;
                options.User.RequireUniqueEmail = siteSettings.IdentitySettings.UserRequireUniqueEmail;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    var secretKey = Encoding.UTF8.GetBytes(siteSettings.JwtSettings.SecretKey);
                    var encryptKey = Encoding.UTF8.GetBytes(siteSettings.JwtSettings.EncryptKey);

                    var validationParameters = new TokenValidationParameters
                    {
                        RequireSignedTokens = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                        TokenDecryptionKey = new SymmetricSecurityKey(encryptKey),
                        ValidateIssuer = true,
                        ValidIssuer = siteSettings.JwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = siteSettings.JwtSettings.Audience,
                        RequireExpirationTime = true,
                        ValidateLifetime = true
                    };

                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = validationParameters;
                    options.SaveToken = true;

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception != null)
                            {
                                throw new AppException(HttpStatusCode.Unauthorized, "Autentication failed.", context.Exception);
                            }

                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            if (context.AuthenticateFailure != null)
                            {
                                throw new AppException(HttpStatusCode.Unauthorized, "Autentication failed.", context.AuthenticateFailure);
                            }

                            throw new AppException(HttpStatusCode.Unauthorized, "You are unauthorized to access this resource.", null);
                        },
                        OnTokenValidated = async context =>
                        {
                            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                            var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<User>>();
                            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();

                            if (!claimsIdentity.Claims.Any())
                            {
                                context.Fail("No claims were found.");
                            }

                            var securityStampClaim = claimsIdentity.FindFirst(new ClaimsIdentityOptions().SecurityStampClaimType);

                            if (securityStampClaim is null)
                            {
                                context.Fail("No security stamp was found.");
                            }

                            var validatedSecurityStamp = await signInManager.ValidateSecurityStampAsync(context.Principal);

                            if (validatedSecurityStamp is null)
                            {
                                context.Fail("Security stamp is not valid.");
                            }

                            var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                            if (userIdClaim is null)
                            {
                                context.Fail("User id claim was not found.");
                            }

                            var userId = userIdClaim.Value;
                            var user = await userManager.FindByIdAsync(userId);

                            if (!user.IsActive)
                            {
                                context.Fail("User is not active.");
                            }

                            user.LastLoginDate = DateTime.Now;
                            await userManager.UpdateAsync(user);
                        }
                    };
                });
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PMS.WebAPI", Version = "v1" });

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      new string[] { }
                    }
                  });
            });
        }
    }
}
