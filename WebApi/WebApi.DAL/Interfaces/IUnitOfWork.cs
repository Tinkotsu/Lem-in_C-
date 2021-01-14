using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApi.DAL.Entities.Material;
using WebApi.DAL.Entities.User;

namespace WebApi.DAL.Interfaces
{
    public interface IUnitOfWork // : IDisposable
    {
        IRepository<Material> Materials { get; }
        IRepository<MaterialVersion> MaterialVersions { get; }
        IRepository<MaterialUser> MaterialUsers { get; }
        RoleManager<IdentityRole> RoleManager { get; }
        UserManager<ApplicationUser> UserManager { get; }
        Task SaveAsync();
    }
}
