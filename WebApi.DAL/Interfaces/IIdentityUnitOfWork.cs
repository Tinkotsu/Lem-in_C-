using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApi.DAL.Entities.User;

namespace WebApi.DAL.Repositories
{
    public interface IIdentityUnitOfWork : IDisposable
    {
        RoleManager<ApplicationRole> RoleManager { get; }
        UserManager<ApplicationUser> UserManager { get; }

        Task SaveAsync();
    }
}