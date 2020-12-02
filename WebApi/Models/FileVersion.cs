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

        [MaxLength(200)]
        public string FilePath { get; set; }

        [Required]
        public int FileSize { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int VersionNumber { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        public File File { get; set; }
        public int FileId { get; set; }
    }
}
