using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApi.DAL.EF;
using WebApi.DAL.Entities.Material;
using WebApi.DAL.Interfaces;

namespace WebApi.DAL.Repositories
{
    public class MaterialUserRepository: IRepository<MaterialUser>
    {
        private readonly MaterialDbContext _db;

        public MaterialUserRepository(MaterialDbContext context)
        {
            _db = context;
        }

        public IEnumerable<MaterialUser> GetAll()
        {
            return _db.MaterialUsers;
        }

        public MaterialUser Get(string id)
        {
            return _db.MaterialUsers.Find(id);
        }

        public void Create(MaterialUser user)
        {
            _db.MaterialUsers.Add(user);
        }

        public void Update(MaterialUser user)
        {
            _db.Entry(user).State = EntityState.Modified;
        }

        public IEnumerable<MaterialUser> Find(Func<MaterialUser, bool> predicate)
        {
            return _db.MaterialUsers.Where(predicate).ToList();
        }

        public void Delete(string id)
        {
            var user = _db.MaterialUsers.Find(id);
            if (user != null)
                _db.MaterialUsers.Remove(user);
        }
    }
}