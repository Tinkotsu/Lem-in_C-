using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using WebApi.BLL.Interfaces;
using WebApi.DAL.Entities.Material;
using WebApi.DAL.Interfaces;
using WebApi.BLL.DTO.Material;
using WebApi.BLL.Infrastructure;

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

        void SaveMaterial(FileDTO fileDto, string userId)
        {
            if (fileDto.FormFile == null)
                throw new ValidationException("No file uploaded", "formFile");
            if (fileDto.CategoryId < 1 || fileDto.CategoryId > 3)
                throw new ValidationException("Wrong category ID", "categoryId");
            
            var hash = HashCalculator.CalculateMD5(fileDto.FormFile);
        }

        void SaveMaterialVersion(int? materialId, MaterialVersionDTO materialVersionDto)
        {

        }

        void EditMaterialCategory(int? materialId, int? newVersionId)
        {
        }

        MaterialDTO GetMaterial(int? id)
        {
        }

        MaterialVersionDTO GetMaterialVersion(int? materialId, int? versionId)
        {
        }

        IEnumerable<MaterialVersionDTO> GetMaterialVersions()
        {
        }

        IEnumerable<MaterialDTO> GetMaterials()
        {
        }

        void Dispose()
        {
        }
    }
}
