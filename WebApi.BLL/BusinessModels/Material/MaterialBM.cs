using System;
using System.Collections.Generic;

namespace WebApi.BLL.BusinessModels.Material
{
    public class MaterialBM
    {
        public string Name { get; set; }

        public int CategoryId { get; set; }

        public int ActualVersionNum { get; set; }

        public string OwnerUserId { get; set; }
    }
}
