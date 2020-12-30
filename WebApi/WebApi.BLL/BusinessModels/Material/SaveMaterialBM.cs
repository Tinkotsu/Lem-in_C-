using Microsoft.AspNetCore.Http;

namespace WebApi.BLL.BusinessModels.Material
{
    public class SaveMaterialBM
    {
        public IFormFile File { get; set; }

        public int CategoryId { get; set; }

        public string UserId { get; set; }
    }
}
