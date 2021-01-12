using System;
namespace WebApi.BLL.BusinessModels.Material
{
    public class MaterialVersionBm
    {
        public string Id { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public int VersionNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
