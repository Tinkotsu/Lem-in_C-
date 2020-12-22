using System;
using System.Collections.Generic;
using System.Text;
using WebApi.BLL.DTO.Material;

namespace WebApi.BLL.Interfaces
{
    public interface IMaterialService
    {
        void SaveMaterial(MaterialDTO materialDto, string userId);
        void SaveMaterialVersion(MaterialVersionDTO materialVersionDto);
        void EditMaterialCategory(int? materialId, int? newVersionId);
        MaterialDTO GetMaterial(int? id);
        MaterialVersionDTO GetMaterialVersion(int? materialId, int? versionId);
        IEnumerable<MaterialVersionDTO> GetMaterialVersions();
        IEnumerable<MaterialDTO> GetMaterials();
        void Dispose();
    }
}
