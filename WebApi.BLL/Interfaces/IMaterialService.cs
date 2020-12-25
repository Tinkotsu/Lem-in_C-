using System;
using System.Collections.Generic;
using System.Text;
using WebApi.BLL.BusinessModels.Material;

namespace WebApi.BLL.Interfaces
{
    public interface IMaterialService
    {
        void SaveMaterial(SaveMaterialBM material);

        void SaveMaterialVersion(SaveMaterialVersionBM materialVersion);

        void EditMaterialCategory(EditCategoryBM editCategoryBM);

        MaterialBM GetMaterial(string fileName, string userId);

        MaterialVersionBM GetMaterialVersion(MaterialBM materialBM);

        IEnumerable<MaterialVersionBM> GetMaterialVersions();

        IEnumerable<MaterialBM> GetMaterials();

        IEnumerable<MaterialBM> GetFilteredMaterials(MaterialsFilterRequestBM materialsFilterRequestBM);

        byte[] GetMaterialFile(GetMaterialFileBM getMaterialFileBM);

       // void Dispose();
    }
}
