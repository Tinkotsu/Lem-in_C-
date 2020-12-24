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
    public class MaterialRepository : IRepository<MaterialDTO>
    {
        private readonly MaterialDbContext _db;

        public MaterialRepository(MaterialDbContext context)
        {
            _db = context;
        }

        public IEnumerable<MaterialDTO> GetAll()
        {
            return _db.Materials;
        }

        public MaterialDTO Get(string id)
        {
            return _db.Materials.Find(int.Parse(id));
        }

        public void Create(MaterialDTO material)
        {
            _db.Materials.Add(material);
        }

        public void Update(MaterialDTO material)
        {
            _db.Entry(material).State = EntityState.Modified;
        }

        public IEnumerable<MaterialDTO> Find(Func<MaterialDTO, bool> predicate)
        {
            return _db.Materials.Include(x => x.Versions).Where(predicate).ToList();
        }

        public void Delete(int id)
        {
            var material = _db.Materials.Find(id);
            if (material != null)
                _db.Materials.Remove(material);
        }
    }
}
