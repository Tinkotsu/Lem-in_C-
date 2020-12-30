using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApi.DAL.EF;
using WebApi.DAL.Entities.Material;
using WebApi.DAL.Interfaces;

namespace WebApi.DAL.Repositories
{
    public class UserMaterialVersionRepository : IRepository<UserMaterialVersion>
    {
        private readonly MaterialDbContext _db;

        public UserMaterialVersionRepository(MaterialDbContext context)
        {
            _db = context;
        }

        public IEnumerable<UserMaterialVersion> GetAll()
        {
            return _db.UserMaterialVersions;
        }

        public UserMaterialVersion Get(string id)
        {
            return _db.UserMaterialVersions.Find(id);
        }

        public void Create(UserMaterialVersion userMaterialVersion)
        {
            _db.UserMaterialVersions.Add(userMaterialVersion);
        }

        public void Update(UserMaterialVersion userMaterialVersion)
        {
            _db.Entry(userMaterialVersion).State = EntityState.Modified;
        }

        public IEnumerable<UserMaterialVersion> Find(Func<UserMaterialVersion, bool> predicate)
        {
            return _db.UserMaterialVersions.Where(predicate).ToList();
        }

        public void Delete(string id)
        {
            var userMaterialVersion = _db.UserMaterialVersions.Find(id);
            if (userMaterialVersion != null)
                _db.UserMaterialVersions.Remove(userMaterialVersion);
        }
    }
}
