using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WebApi.DAL.EF;
using WebApi.DAL.Entities.Material;
using WebApi.DAL.Interfaces;

namespace WebApi.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MaterialDbContext _db;
        private MaterialRepository _materialRepository;
        private MaterialVersionRepository _materialVersionRepository;

        public UnitOfWork(MaterialDbContext context)
        {
            _db = context;
        }

        public IRepository<Material> Materials
        {
            get
            {
                _materialRepository ??= new MaterialRepository(_db);
                return _materialRepository;
            }
        }
        public IRepository<MaterialVersion> MaterialVersions
        {
            get
            {
                _materialVersionRepository ??= new MaterialVersionRepository(_db);
                return _materialVersionRepository;
            }
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        private bool _disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _db.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
