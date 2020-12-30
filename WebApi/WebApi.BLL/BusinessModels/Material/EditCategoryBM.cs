using System;
namespace WebApi.BLL.BusinessModels.Material
{
    public class EditCategoryBM
    {
        public string FileName { get; set; }

        public int? NewCategoryID { get; set; }

        public string UserId { get; set; }
    }
}