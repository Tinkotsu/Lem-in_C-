using System;
using System.Collections.Generic;
using System.Text;
using WebApi.DAL.Entities.Material;
using WebApi.DAL.Entities.User;

namespace WebApi.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<MaterialDTO> Materials { get; }
        IRepository<MaterialVersionDTO> MaterialVersions { get; }
        void Save();
    }
}
