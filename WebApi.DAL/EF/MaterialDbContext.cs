using System;
using Microsoft.EntityFrameworkCore;
using WebApi.DAL.Entities.Material;

namespace WebApi.DAL.EF
{

    public class MaterialDbContext : DbContext
    {
        public MaterialDbContext()
        { }

        public MaterialDbContext(DbContextOptions<MaterialDbContext> options)
            : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseMySql("server=localhost;port=3306;user=root;password=Ryuzaki12;database=SenatTestApiDb", new MySqlServerVersion(new Version(8, 0, 22)))
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            modelBuilder.Entity<Material>()
                .HasMany<MaterialVersion>(x => x.Versions)
                .WithOne(x => x.Material)
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<Material> Materials { get; set; }
        public DbSet<MaterialVersion> MaterialVersions { get; set; }
    }
}
