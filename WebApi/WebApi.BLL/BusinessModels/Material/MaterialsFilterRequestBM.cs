using System;
namespace WebApi.BLL.BusinessModels.Material
{
    public class MaterialsFilterRequestBM
    {
        public int? MinSize { get; set; }

        public int? MaxSize { get; set; }

        public int? CategoryId { get; set; }
    }
}
