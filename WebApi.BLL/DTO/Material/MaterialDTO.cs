using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.BLL.DTO.Material
{
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
    }
}
