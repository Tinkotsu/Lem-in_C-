﻿using System;
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
        private readonly IMapper _mapper;

        public MaterialService(IUnitOfWork uow, IFileManager fileManager, IMapper mapper)
        {
            _unitOfWork = uow;
            _fileManager = fileManager;
            _mapper = mapper;
        }

        public void SaveMaterial(MaterialBm material, MaterialFileBm file)
        {
            if (file?.FileBytes == null)
                throw new ValidationException("No file uploaded", "file");
            if (material.Category == null)
                throw new ValidationException("Wrong category", "category");
            if (material.OwnerUserId == null)
                throw new ValidationException("Wrong user ID", "user ID");
            
            var hash = HashCalculator.CalculateMd5(file.FileBytes);

            var identicalMaterialVersions = _unitOfWork.MaterialVersions.Find(v => v.FilePath == hash).ToList();
            
            //saving file locally

            var path = identicalMaterialVersions.FirstOrDefault()?.FilePath;
            path ??= _fileManager.SaveFile(file.FileBytes, hash).Result;

            //creating material & version

            var materialDb = new Material
            {
                Name = file.FileName,
                ActualVersionNum = 1,
                Category = (MaterialCategories) material.Category,
                OwnerUserId = material.OwnerUserId,
                Versions = new List<MaterialVersion>()
            };

            var materialVersionDb = new MaterialVersion
            {
                Id = Guid.NewGuid().ToString(),
                FileSize = file.FileSize,
                FilePath = path,
                Material = materialDb,
                VersionNumber = 1,
                CreatedAt = DateTime.Now
            };

            var materialUser = _unitOfWork.MaterialUsers.Find(u => u.Id == material.OwnerUserId).FirstOrDefault();

            if (materialUser == null)
            {
                materialUser = new MaterialUser
                {
                    Id = material.OwnerUserId,
                    Materials = new List<Material>()
                };
                _unitOfWork.MaterialUsers.Create(materialUser);
            }

            materialDb.Versions.Add(materialVersionDb);
            materialUser.Materials.Add(materialDb);
            
            //saving material and material version to db

            _unitOfWork.MaterialVersions.Create(materialVersionDb);
            _unitOfWork.Materials.Create(materialDb);
            _unitOfWork.SaveAsync();
        }

        public void SaveMaterialVersion(MaterialFileBm file, string userId, bool isActual)
        {
            if (file?.FileBytes == null)
                throw new ValidationException("No file uploaded", "file");

            //saving file locally

            var hash = HashCalculator.CalculateMd5(file.FileBytes);

            var identicalMaterialVersions = _unitOfWork.MaterialVersions.Find(v => v.FilePath == hash).ToList();
            
            var materialUser = _unitOfWork.MaterialUsers.Get(userId);
            if (materialUser == null)
                throw new NotFoundException("User has not been found", "UserId");

            var materialDb = materialUser.Materials.FirstOrDefault(m => m.Name == file.FileName);
            if (materialDb == null)
                throw new NotFoundException("Material with the name given has not been found", "fileName");

            var path = identicalMaterialVersions.FirstOrDefault()?.FilePath;
            path ??= _fileManager.SaveFile(file.FileBytes, hash).Result;
            
            //saving material version to db

            var newVersionNum = _unitOfWork.MaterialVersions.Find(x => x.MaterialId == materialDb.Id).Count() + 1;
            
            var materialVersionDb = new MaterialVersion
            {
                Id = Guid.NewGuid().ToString(),
                FileSize = file.FileSize,
                FilePath = path,
                Material = materialDb,
                VersionNumber = newVersionNum,
                CreatedAt = DateTime.Now
            };

            if (isActual)
                materialDb.ActualVersionNum = newVersionNum;

            materialDb.Versions.Add(materialVersionDb);
            materialUser.Materials.Add(materialDb);
            
            //saving material and material version to db

            _unitOfWork.MaterialVersions.Create(materialVersionDb);
            _unitOfWork.SaveAsync();
        }

        public void EditMaterialCategory(MaterialBm materialBm)
        {
            if (materialBm?.Name == null)
                throw new ValidationException("Material file name must be set", "FileName");
            if (materialBm.Category == null)
                throw new ValidationException("Category must be set", "Category");
            
            var materialDb = _unitOfWork.Materials
                .Find(material => material.OwnerUserId == materialBm.OwnerUserId && material.Name == materialBm.Name)
                .FirstOrDefault();
            if (materialDb == null)
                throw new ValidationException("Material ID is not valid", "MaterialId");

            materialDb.Category = (MaterialCategories) materialBm.Category;
            _unitOfWork.SaveAsync();
        }

        public MaterialBm GetMaterial(string fileName, string userId)
        {
            if (fileName == null)
                throw new ValidationException("File name must be set", "fileName");

            var material = _unitOfWork.Materials
                .Find(m => m.Name == fileName && m.OwnerUserId == userId)
                .FirstOrDefault();

            if (material == null)
                throw new NotFoundException("Material has not been found", "");

            return _mapper.Map<Material, MaterialBm>(material);
        }

        public MaterialVersionBm GetMaterialVersion(MaterialBm materialBm)
        {
            var material = _unitOfWork.Materials
                .Find(m => m.Name == materialBm.Name && m.OwnerUserId == materialBm.OwnerUserId)
                .FirstOrDefault();

            if (material == null)
                throw new NotFoundException("Material has not been found", "");

            var materialVersion = material.Versions.FirstOrDefault(version => version.VersionNumber == material.ActualVersionNum);

            return _mapper.Map<MaterialVersion, MaterialVersionBm>(materialVersion);
        }

        public IEnumerable<MaterialVersionBm> GetMaterialVersions()
        {
            var materialVersionsDb = _unitOfWork.MaterialVersions.GetAll();

            return _mapper.Map<IEnumerable<MaterialVersion>, List<MaterialVersionBm>>(materialVersionsDb);
        }

        public IEnumerable<MaterialBm> GetMaterials()
        {
            var materialsDb = _unitOfWork.Materials.GetAll();

            return _mapper.Map<IEnumerable<Material>, List<MaterialBm>>(materialsDb);
        }

        public byte[] GetMaterialFile(MaterialBm materialBm)
        {
            if (materialBm?.Name == null)
                throw new ValidationException("File name must be set", "FileName");

            var material = _unitOfWork.Materials
                .Find(m => m.OwnerUserId == materialBm.OwnerUserId && m.Name == materialBm.Name).FirstOrDefault();

            if (material == null)
                throw new NotFoundException("File can not be found", "");

            var materialVersionNum = materialBm.ActualVersionNum ?? material.ActualVersionNum;

            var materialVersion = material.Versions.FirstOrDefault(v => v.VersionNumber == materialVersionNum);
            if (materialVersion == null)
                throw new NotFoundException("Version has not been found", "");

            return _fileManager.GetFile(materialVersion.Id);
        }

        public IEnumerable<MaterialBm> GetFilteredMaterials(MaterialBm materialBm, long? minSize, long? maxSize)
        {

            if (minSize != null && maxSize != null)
            {
                if (maxSize < minSize || maxSize < 0 || minSize < 0)
                    throw new ValidationException("Wrong min or max size", "size");
            }

            minSize ??= 0;
            maxSize ??= -1;

            var materials = _unitOfWork.Materials.GetAll();

            if (materialBm.Category != null)
                materials = materials.Where(m => m.Category == materialBm.Category);
            
            if (maxSize != -1 || minSize != 0)
            {
                materials = materials
                    .Where(material =>
                    {
                        var versionNum = material.ActualVersionNum;
                        var version = material.Versions.FirstOrDefault(v => v.VersionNumber == versionNum);
                        return (version?.FileSize >= minSize && (maxSize == -1 || version.FileSize <= maxSize));
                    })
                    .ToList();
            }

            return _mapper.Map<IEnumerable<Material>, List<MaterialBm>>(materials);
        }

        //public void Dispose()
        //{
        //    _unitOfWork.Dispose();
        //}
    }
}
