using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using WebApi.DAL.Entities.Material;
using WebApi.DAL.Entities.User;

namespace WebApi.DAL.Interfaces
{
    public interface IUnitOfWork // : IDisposable
    {
        IRepository<Material> Materials { get; }
        IRepository<MaterialVersion> MaterialVersions { get; }
        IRepository<UserMaterialVersion> UserMaterialVersions { get; }
        void Save();
    }
}
