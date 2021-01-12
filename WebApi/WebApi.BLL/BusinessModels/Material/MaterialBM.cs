using System;
using System.Collections.Generic;
using WebApi.DAL.Entities.Material;

namespace WebApi.BLL.BusinessModels.Material
{
    public class MaterialBm
    {
        public string Name { get; set; }
        public MaterialCategories? Category { get; set; }
        public int? ActualVersionNum { get; set; }
        public string OwnerUserId { get; set; }
    }
}
