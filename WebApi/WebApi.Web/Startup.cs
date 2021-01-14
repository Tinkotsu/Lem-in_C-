using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi.BLL.Interfaces;
using WebApi.BLL.Services;
using WebApi.DAL.EF;
using WebApi.DAL.Entities.User;
using WebApi.DAL.Interfaces;
using WebApi.DAL.Repositories;

namespace WebApi.Web
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
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("WebApi.DAL")));

            services.AddDbContext<MaterialDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("WebApi.DAL")));

            //Identity settings
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
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


            //Global mapper config
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            //DI for logger
            services.AddSingleton<ILogStorage, FileLogStorage>();

            //DI for Unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //DI for file management
            services.AddSingleton<IFileManager, FileManager>();

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
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = (c) =>
                {
                    var exception = c.Features.Get<IExceptionHandlerFeature>();
                    var statusCode = exception.Error.GetType().Name switch
                    {
                        "ValidationException" => HttpStatusCode.BadRequest,
                        "NotFoundException" => HttpStatusCode.NotFound,
                        _ => HttpStatusCode.ServiceUnavailable
                    };
                    c.Response.StatusCode = (int) statusCode;
                    
                    return Task.CompletedTask;
                }
            });

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