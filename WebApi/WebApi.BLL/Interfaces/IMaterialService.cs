using System;
using System.Collections.Generic;
using System.Text;
using WebApi.BLL.BusinessModels.Material;

namespace WebApi.BLL.Interfaces
{
    public interface IMaterialService
    {
        void SaveMaterial(MaterialBm material, MaterialFileBm file);

        void SaveMaterialVersion(MaterialFileBm file, string userId, bool isActual);

        void EditMaterialCategory(MaterialBm materialBm);

        MaterialBm GetMaterial(string fileName, string userId);

        MaterialVersionBm GetMaterialVersion(MaterialBm materialBm);

        IEnumerable<MaterialVersionBm> GetMaterialVersions();

        IEnumerable<MaterialBm> GetMaterials();

        IEnumerable<MaterialBm> GetFilteredMaterials(MaterialBm materialBm, long? minSize, long? maxSize);

        byte[] GetMaterialFile(MaterialBm materialBm);

        // void Dispose();
    }
}
