using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using WebApi.Models;

namespace WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //var configuration = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.json")
            //    .Build();

            //Log.Logger = new LoggerConfiguration()
            //    .ReadFrom.Configuration(configuration)
            //    .CreateLogger();


            var host = CreateHostBuilder(args).Build();
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await RoleInitializer.InitializeAsync(userManager, rolesManager);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while initializing roles: {ex.Message}");
                }
            }

            host.Run();
            //catch (Exception ex)
            //{
            //    Log.Fatal(ex, "The application failed to start.");
            //}
            //finally
            //{
            //    Log.CloseAndFlush();
            //}
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.UseSerilog()
                //.ConfigureLogging((context, logging) =>
                //{
                //    logging.ClearProviders();
                //    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                //    logging.AddDebug();
                //    logging.AddConsole();
                //})
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
