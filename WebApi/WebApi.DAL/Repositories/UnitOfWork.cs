using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WebApi.DAL.EF;
using WebApi.DAL.Entities.Material;
using WebApi.DAL.Interfaces;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApi.DAL.Entities.User;

namespace WebApi.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserDbContext _userDbContext;
        private readonly MaterialDbContext _materialDb;
        private MaterialRepository _materialRepository;
        private MaterialVersionRepository _materialVersionRepository;
        private MaterialUserRepository _materialUserRepository;
        public UserManager<ApplicationUser> UserManager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }
        
        public UnitOfWork(
            MaterialDbContext materialDbContext,
            UserDbContext userDbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _materialDb = materialDbContext;
            _userDbContext = userDbContext;
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public IRepository<Material> Materials
        {
            get
            {
                _materialRepository ??= new MaterialRepository(_materialDb);
                return _materialRepository;
            }
        }
        public IRepository<MaterialVersion> MaterialVersions
        {
            get
            {
                _materialVersionRepository ??= new MaterialVersionRepository(_materialDb);
                return _materialVersionRepository;
            }
        }

        public IRepository<MaterialUser> MaterialUsers
        {
            get
            {
                _materialUserRepository ??= new MaterialUserRepository(_materialDb);
                return _materialUserRepository;
            }
        }


        public async Task SaveAsync()
        {
            await _materialDb.SaveChangesAsync();
        }

        //private bool _disposed = false;

        //public virtual void Dispose(bool disposing)
        //{
        //    if (_disposed) return;
        //    if (disposing)
        //    {
        //        _db.Dispose();
        //    }
        //    _disposed = true;
        //}

        //public void Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}
    }
}
