using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.DAL.Entities.Material
{
    public class UserMaterialVersion
    {
        public UserMaterialVersion()
        {
            this.MaterialVersions = new HashSet<MaterialVersion>();
        }

        [Key]
        public string UserId { get; set; }

        public virtual ICollection<MaterialVersion> MaterialVersions { get; set; }
    }
}
