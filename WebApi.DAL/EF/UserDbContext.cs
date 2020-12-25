using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApi.DAL.Entities.User;

namespace WebApi.DAL.EF
{
    public class UserDbContext : IdentityDbContext<ApplicationUser>
    {
        public UserDbContext()
        { }

        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseMySql("server=localhost;port=3306;user=root;password=Ryuzaki12;database=SenatTestApiDb", new MySqlServerVersion(new Version(8, 0, 22)))
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }
    }
}
