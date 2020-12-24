using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.DAL.Entities.Material
{
    public enum MaterialCategories
    {
        Presentation = 1,
        Application = 2,
        Other = 3
    }

    public class MaterialDTO
    {

        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int ActualVersionNum { get; set; }

        [Required]
        public string OwnerUserId { get; set; }

        public ICollection<MaterialVersionDTO> Versions { get; set; }
    }
}
