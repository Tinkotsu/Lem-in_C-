using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.AppData
{


    public class MaterialDbContext : DbContext
    {

        public MaterialDbContext(DbContextOptions<MaterialDbContext> options)
            : base(options)
        { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            modelBuilder.Entity<Material>()
                .HasMany<MaterialVersion>(x => x.Versions)
                .WithOne(x => x.Material)
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<FileVersion>()
            //    .Property(f => f.CreatedAt)
            //    .HasDefaultValueSql("getdate()");

            //modelBuilder.Entity<MaterialCategory>().HasData(
            //    new MaterialCategory() { Id = 1, Name = "Presentation" },
            //    new MaterialCategory() { Id = 2, Name = "Application" },
            //    new MaterialCategory() { Id = 3, Name = "Other" });
        }

        public DbSet<Material> Materials { get; set; }
        public DbSet<MaterialVersion> MaterialVersions { get; set; }
    }
}
