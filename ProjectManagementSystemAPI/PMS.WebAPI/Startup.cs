using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using PMS.Common;
using PMS.Data;
using PMS.Data.Repositories;
using PMS.Entities;
using PMS.Services;
using PMS.Services.Implementations;
using PMS.WebFramework.AutoMapperProfile;
using PMS.WebFramework.Configurations;
using PMS.WebFramework.CustomMiddlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMS.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlServer"));
            });

            services.Configure<SiteSettings>(Configuration.GetSection(nameof(SiteSettings)));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IJwtServices, JwtServices>();

            services.AddAutoMapper(config => config.AddProfile(new AutoMapperProfile()));

            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            SiteSettings siteSettings = Configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();

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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PMS.WebAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("PMS");

            app.UseMiddleware<CustomExceptionHandlerMiddleware>();

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PMS.WebAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
