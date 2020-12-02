using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class FileVersion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string FilePath { get; set; }

        [Required]
        public int FileSize { get; set; }

        [Required]
        public int VersionId { get; set; }
        
        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public int FileId { get; set; }
    }
}
