using Microsoft.EntityFrameworkCore;
using WebApi.DAL.Entities.Material;

namespace WebApi.DAL.EF
{

    public class MaterialDbContext : DbContext
    {

        public MaterialDbContext(DbContextOptions<MaterialDbContext> options)
            : base(options)
        { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            modelBuilder.Entity<MaterialDTO>()
                .HasMany<MaterialVersionDTO>(x => x.Versions)
                .WithOne(x => x.Material)
                .HasForeignKey(x => x.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<MaterialDTO> Materials { get; set; }
        public DbSet<MaterialVersionDTO> MaterialVersions { get; set; }
    }
}
