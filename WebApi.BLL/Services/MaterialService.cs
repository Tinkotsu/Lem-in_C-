using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.BLL.Interfaces;
using WebApi.DAL.Entities.Material;
using WebApi.DAL.Interfaces;
using WebApi.BLL.Infrastructure;
using AutoMapper;
using WebApi.BLL.BusinessModels.Material;

namespace WebApi.BLL.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileManager _fileManager;

        public MaterialService(IUnitOfWork uow, IFileManager fileManager)
        {
            _unitOfWork = uow;
            _fileManager = fileManager;
        }

        public void SaveMaterial(SaveMaterialBM material)
        {
            if (material.File == null)
                throw new ValidationException("No file uploaded", "formFile");
            if (material.CategoryId < 1 || material.CategoryId > 3)
                throw new ValidationException("Wrong category ID", "categoryId");
            if (material.UserId == null)
                throw new ValidationException("Wrong user ID", "user ID");
            
            var hash = HashCalculator.CalculateMD5(material.File);

            var identicalMaterials = _unitOfWork.MaterialVersions.Find(v => v.Id == hash);

            if (identicalMaterials.Any(v => v.OwnerUserId == material.UserId))
                throw new ValidationException("File already exists", "");

            //saving file locally
            string path = null;

            if (identicalMaterials.Any())
                path = identicalMaterials.FirstOrDefault().FilePath;
            else
                path = _fileManager.SaveFile(material.File, hash).Result;

            //creating material & version DTOs

            MaterialDTO materialDTO = new MaterialDTO
            {
                Name = material.File.FileName,
                ActualVersionNum = 1,
                CategoryId = material.CategoryId,
                OwnerUserId = material.UserId,
                Versions = new List<MaterialVersionDTO>()
            };

            MaterialVersionDTO materialVersionDTO = new MaterialVersionDTO
            {
                FileSize = material.File.Length,
                FilePath = path,
                Material = materialDTO,
                VersionNumber = 1,
                CreatedAt = DateTime.Now,
                OwnerUserId = material.UserId
            };

            //saving material and material version to db

            _unitOfWork.MaterialVersions.Create(materialVersionDTO);
            _unitOfWork.Materials.Create(materialDTO);
            _unitOfWork.Save();
        }

        public void SaveMaterialVersion(SaveMaterialVersionBM materialVersion)
        {
            if (materialVersion.MaterialId == null)
                throw new ValidationException("Material ID must be set", "materialId");
            if (materialVersion.File == null)
                throw new ValidationException("No file uploaded", "formFile");
            if (materialVersion.UserId == null)
                throw new ValidationException("Wrong user ID", "user ID");


            //saving file locally

            var hash = HashCalculator.CalculateMD5(materialVersion.File);

            var identicalMaterials = _unitOfWork.MaterialVersions.Find(v => v.Id == hash);

            if (identicalMaterials.Any(v => v.OwnerUserId == materialVersion.UserId))
                throw new ValidationException("File already exists", "");

            var materialDTO = _unitOfWork.Materials.Get(materialVersion.MaterialId.ToString());
            if (materialDTO == null)
                throw new ValidationException("Material ID is not valid", "materialId");

            string path = null;

            if (identicalMaterials.Any())
                path = identicalMaterials.FirstOrDefault().FilePath;
            else
                path = _fileManager.SaveFile(materialVersion.File, hash).Result;

            //saving material version to db

            var newVersionNum = _unitOfWork.MaterialVersions.Find(x => x.MaterialId == materialDTO.Id).Count() + 1;

            MaterialVersionDTO materialVersionDTO = new MaterialVersionDTO
            {
                FileSize = materialVersion.File.Length,
                FilePath = path,
                Material = materialDTO,
                VersionNumber = newVersionNum,
                CreatedAt = DateTime.Now,
                OwnerUserId = materialVersion.UserId
            };
            if (materialVersion.IsActual == true)
                materialDTO.ActualVersionNum = newVersionNum;

            _unitOfWork.MaterialVersions.Create(materialVersionDTO);
            _unitOfWork.Save();
        }

        public void EditMaterialCategory(EditCategoryBM editCategoryBM)
        {
            if (editCategoryBM.FileName == null)
                throw new ValidationException("Material file name must be set", "FileName");
            if (editCategoryBM.NewCategoryID == null)
                throw new ValidationException("New category ID must be set", "NewCategoryId");
            if (editCategoryBM.NewCategoryID < 1 || editCategoryBM.NewCategoryID > 3)
                throw new ValidationException("Wrong category ID", "NewCategoryId");

            var materialDTO = _unitOfWork.Materials
                .Find(material => material.OwnerUserId == editCategoryBM.UserId && material.Name == editCategoryBM.FileName)
                .FirstOrDefault();
            if (materialDTO == null)
                throw new ValidationException("Material ID is not valid", "MaterialId");

            materialDTO.CategoryId = editCategoryBM.NewCategoryID.Value;
            _unitOfWork.Save();
        }

        public MaterialBM GetMaterial(string fileName, string userId)
        {
            if (fileName == null)
                throw new ValidationException("File name must be set", "fileName");

            var material = _unitOfWork.Materials
                .Find(material => material.Name == fileName && material.OwnerUserId == userId)
                .FirstOrDefault();

            if (material == null)
                throw new ValidationException("Material has not been found", "");

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<MaterialDTO, MaterialBM>()).CreateMapper();

            return mapper.Map<MaterialDTO, MaterialBM>(material);
        }

        public MaterialVersionBM GetMaterialVersion(MaterialBM materialBM)
        {
            var material = _unitOfWork.Materials
                .Find(material => material.Name == materialBM.Name && material.OwnerUserId == materialBM.OwnerUserId)
                .FirstOrDefault();

            if (material == null)
                throw new ValidationException("Material has not been found", "");

            var materialVersion = material.Versions.FirstOrDefault(version => version.VersionNumber == material.ActualVersionNum);

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<MaterialVersionDTO, MaterialVersionBM>()).CreateMapper();

            return mapper.Map<MaterialVersionDTO, MaterialVersionBM>(materialVersion);
        }

        public IEnumerable<MaterialVersionBM> GetMaterialVersions()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<MaterialVersionDTO, MaterialVersionBM>()).CreateMapper();
            return mapper.Map<IEnumerable<MaterialVersionDTO>, List<MaterialVersionBM>>(_unitOfWork.MaterialVersions.GetAll());
        }

        public IEnumerable<MaterialBM> GetMaterials()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<MaterialDTO, MaterialBM>()).CreateMapper();
            return mapper.Map<IEnumerable<MaterialDTO>, List<MaterialBM>>(_unitOfWork.Materials.GetAll());
        }

        public byte[] GetMaterialFile(GetMaterialFileBM getMaterialFileBM)
        {
            if (getMaterialFileBM.FileName == null)
                throw new ValidationException("File name must be set", "FileName");

            var material = _unitOfWork.Materials
                .Find(material => material.OwnerUserId == getMaterialFileBM.UserId && material.Name == getMaterialFileBM.FileName).FirstOrDefault();

            if (material == null)
                throw new ValidationException("File can not be found", "");

            int materialVersionNum = getMaterialFileBM.VersionNumber == null ? material.ActualVersionNum : getMaterialFileBM.VersionNumber.Value;

            var materialVersion = material.Versions.FirstOrDefault(v => v.VersionNumber == materialVersionNum);
            if (materialVersion == null)
                throw new ValidationException("Version has not been found", "");

            return _fileManager.GetFile(materialVersion.Id);
        }

        public IEnumerable<MaterialBM> GetFilteredMaterials(MaterialsFilterRequestBM materialsFilterRequestBM)
        {
            var minSize = materialsFilterRequestBM.MinSize;
            var maxSize = materialsFilterRequestBM.MaxSize;
            var categoryId = materialsFilterRequestBM.CategoryId;

            if (minSize != null && maxSize != null)
            {
                if (maxSize < minSize || maxSize < 0 || minSize < 0)
                    throw new ValidationException("Wrong min or max size", "size");
            }

            minSize ??= 0;
            maxSize ??= -1;

            var materials = _unitOfWork.Materials.Find(material => true);

            if (categoryId != 0)
            {
                if (categoryId < 1 || categoryId > 3)
                    throw new ValidationException("Wrong category ID", "categoryID");
                materials = materials.Where(material => material.CategoryId == categoryId);
            }

            if (maxSize != -1 || minSize != 0)
            {
                materials = materials
                    .Where(material =>
                    {
                        var versionNum = material.ActualVersionNum;
                        var version = material.Versions.FirstOrDefault(v => v.VersionNumber == versionNum);
                        return (version.FileSize >= minSize && (maxSize == -1 || version.FileSize <= maxSize));
                    })
                    .ToList();
            }

            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<MaterialDTO, MaterialBM>()).CreateMapper();
            return mapper.Map<IEnumerable<MaterialDTO>, List<MaterialBM>>(materials);
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }
    }
}
