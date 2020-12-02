using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class File
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public virtual FileCategory Category { get; set; }
        
        [Required]
        public int CategoryId { get; set; }

        public ICollection<FileVersion> Versions { get; set; }
    }
}
