using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.DAL.Entities.Material
{
    public class MaterialUser
    {
        [Key]
        public string Id { get; set; }
        public ICollection<Material> Materials { get; set; }
    }
}