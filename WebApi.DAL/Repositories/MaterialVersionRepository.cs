using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApi.DAL.EF;
using WebApi.DAL.Entities.Material;
using WebApi.DAL.Interfaces;

namespace WebApi.DAL.Repositories
{
    public class MaterialVersionRepository : IRepository<MaterialVersion>
    {
        private readonly MaterialDbContext _db;

        public MaterialVersionRepository(MaterialDbContext context)
        {
            _db = context;
        }

        public IEnumerable<MaterialVersion> GetAll()
        {
            return _db.MaterialVersions;
        }

        public MaterialVersion Get(int id)
        {
            return _db.MaterialVersions.Find(id);
        }

        public void Create(MaterialVersion version)
        {
            _db.MaterialVersions.Add(version);
        }

        public void Update(MaterialVersion version)
        {
            _db.Entry(version).State = EntityState.Modified;
        }

        public IEnumerable<MaterialVersion> Find(Func<MaterialVersion, Boolean> predicate)
        {
            return _db.MaterialVersions.Where(predicate).ToList();
        }

        public void Delete(int id)
        {
            var version = _db.MaterialVersions.Find(id);
            if (version != null)
                _db.MaterialVersions.Remove(version);
        }
    }
}