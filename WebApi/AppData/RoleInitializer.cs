using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApi.Models;

namespace WebApi.AppData
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            var adminEmail = "admin@gmail.com";
            var adminUserName = "admin";
            var password = "_Aa123456";

            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("reader") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("reader"));
            }
            if (await roleManager.FindByNameAsync("initiator") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("initiator"));
            }
            if (await userManager.FindByNameAsync(adminUserName) == null)
            {
                var admin = new User { Email = adminEmail, UserName = adminUserName, Name = adminUserName };
                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                    await userManager.AddToRoleAsync(admin, "initiator");
                }
            }
        }
    }
}
