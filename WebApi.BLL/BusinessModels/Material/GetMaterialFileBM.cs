using System;
namespace WebApi.BLL.BusinessModels.Material
{
    public class GetMaterialFileBM
    {
        public string UserId { get; set; }

        public string FileName { get; set; }

        public int? VersionNumber { get; set; }
    }
}
