using System;

namespace WebApi.BLL.DTO.Material
{
    public class MaterialVersionDTO
    {
        public int Id { get; set; }

        public string FilePath { get; set; }

        public long FileSize { get; set; }

        public int VersionNumber { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public int MaterialId { get; set; }
    }
}
