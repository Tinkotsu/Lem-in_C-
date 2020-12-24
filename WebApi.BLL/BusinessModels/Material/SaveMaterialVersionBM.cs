using System;
using Microsoft.AspNetCore.Http;

namespace WebApi.BLL.BusinessModels.Material
{
    public class SaveMaterialVersionBM
    {
        public int? MaterialId { get; set; }

        public IFormFile File { get; set; }

        public bool IsActual { get; set; }

        public string UserId { get; set; }
    }
}
