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
    public class MaterialRepository : IRepository<Material>
    {
        private readonly MaterialDbContext _db;

        public MaterialRepository(MaterialDbContext context)
        {
            _db = context;
        }

        public IEnumerable<Material> GetAll()
        {
            return _db.Materials;
        }

        public Material Get(string id)
        {
            return _db.Materials.Find(int.Parse(id));
        }

        public void Create(Material material)
        {
            _db.Materials.Add(material);
        }

        public void Update(Material material)
        {
            _db.Entry(material).State = EntityState.Modified;
        }

        public IEnumerable<Material> Find(Func<Material, bool> predicate)
        {
            return _db.Materials.Include(x => x.Versions).Where(predicate).ToList();
        }

        public void Delete(string id)
        {
            var material = _db.Materials.Find(int.Parse(id));
            if (material != null)
                _db.Materials.Remove(material);
        }
    }
}
