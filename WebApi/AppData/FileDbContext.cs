using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.AppData
{
    public class FileDbContext : DbContext
    {
        public DbSet<File> Files { get; set; }
        public DbSet<FileVersion> FileVersions { get; set; }
        public DbSet<FileCategory> FileCategories { get; set; }

        public FileDbContext(DbContextOptions<FileDbContext> options)
            : base(options)
        { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            
            //modelBuilder.Entity<FileVersion>()
            //    .Property(f => f.VersionNumber)
            //    .HasDefaultValue(1)
            //    .ValueGeneratedOnAdd();

            //modelBuilder.Entity<FileVersion>()
            //    .Property(f => f.CreatedAt)
            //    .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<FileCategory>().HasData(
                new FileCategory() { Id = 1, Name = "Презентация" },
                new FileCategory() { Id = 2, Name = "Приложение" },
                new FileCategory() { Id = 3, Name = "Другое" });
        }
    }
}
