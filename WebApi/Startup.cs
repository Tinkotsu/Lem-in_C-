using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using WebApi.DAL.Interfaces;
using WebApi.DAL.Repositories;
using WebApi.Models;
using WebApi.BLL.Interfaces;
using WebApi.BLL.Services;
using WebApi.DAL.EF;
using WebApi.DAL.Entities.User;

namespace WebApi
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
            services.AddControllers();

            //DB settings 
            services.AddDbContext<UserDbContext>(options =>
                options.UseMySQL(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<MaterialDbContext>(options =>
                options.UseMySQL(Configuration.GetConnectionString("DefaultConnection")));

            //Identity settings
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1d);
                options.Lockout.MaxFailedAccessAttempts = 5;
            }).AddEntityFrameworkStores<UserDbContext>();

            //redirect 
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/api/users/login";
                options.AccessDeniedPath = "/api/users/forbidden";
            });


            ////redirect if you use cookie without Identity
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie(options => //CookieAuthenticationOptions
            //    {
            //        options.AccessDeniedPath = "/api/users/login";
            //        options.LoginPath = "/api/users/login";
            //    });

            ////authorize block - FallBackPolicy method
            //services.AddAuthorization(options =>
            //{
            //    options.FallbackPolicy = new AuthorizationPolicyBuilder()
            //        .RequireAuthenticatedUser()
            //        .Build();
            //});


            //DI for logger
            services.AddSingleton<ILogStorage, FileLogStorage>();

            //DI for Unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //DI for Identity Unit of work
            services.AddScoped<IIdentityUnitOfWork, IdentityUnitOfWork>();

            //DI for file management
            services.AddScoped<IFileManager, FileManager>();

            //DI for material management
            services.AddScoped<IMaterialService, MaterialService>();

            //DI for User management
            services.AddScoped<IUserService, UserService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<LoggerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
            });
        }   
    }
}
