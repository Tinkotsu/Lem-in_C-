using System;
namespace WebApi.BLL.BusinessModels.Material
{
    public class MaterialVersionBM
    {
        public string Id { get; set; }

        public string FilePath { get; set; }

        public long FileSize { get; set; }

        public int VersionNumber { get; set; }

        public DateTime CreatedAt { get; set; }

        public string OwnerUserId { get; set; }

        public MaterialBM Material { get; set; }

        public int MaterialId { get; set; }
    }
}
