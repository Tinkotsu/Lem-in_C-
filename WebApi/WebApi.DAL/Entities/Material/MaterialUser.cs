using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace WebApi.DAL.Entities.Material
{
    public class MaterialUser
    {
        [Key]
        public string Id { get; set; }
        public ICollection<Material> Materials { get; set; }
        public ICollection<MaterialVersion> MaterialVersions { get; set; }
    }
}