using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.DAL.Entities.Material
{
    public class MaterialVersionDTO
    {
        [Key]
        public string Id { get; set; }

        [MaxLength(200)]
        public string FilePath { get; set; }

        [Required]
        public long FileSize { get; set; }

        public int VersionNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        public string OwnerUserId { get; set; }

        public MaterialDTO Material { get; set; }

        public int MaterialId { get; set; }
    }
}
