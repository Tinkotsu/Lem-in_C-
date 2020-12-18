using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.BLL.DTO.Material
{
    public class MaterialVersionDTO
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string FilePath { get; set; }

        [Required]
        public long FileSize { get; set; }

        public int VersionNumber { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public int MaterialId { get; set; }
    }
}
