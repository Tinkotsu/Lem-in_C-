using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using WebApi.AppData;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialController : ControllerBase
    {
        //private readonly IFileManager _fileManager;

        private readonly MaterialDbContext _dbContext;
        private readonly IFileManager _fileManager;

        public MaterialController(MaterialDbContext dbContext, IFileManager fileManager)
        {
            _dbContext = dbContext;
            _fileManager = fileManager;
        }

        [HttpPost]
        [Route("add_new_material")]
        public async Task<IActionResult> AddNewFile([FromForm] IFormFile file, [FromForm] int categoryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (file == null)
                return BadRequest("No file uploaded");
            if (categoryId < 1 || categoryId > 3)
                return BadRequest("Wrong category ID");
            if (_dbContext.Materials.Any())
            {
                if (_dbContext.Materials
                    .Count(dbFile => dbFile.OwnerUserId == userId && dbFile.Name == file.FileName) != 0)
                    return BadRequest("File already exist");
            }

            //saving file
            var path = _fileManager.SaveFile(file, userId, 1).Result;

            //creating new material dto
            var material = new Material
            {
                Name = file.FileName,
                ActualVersionNum = 1,
                CategoryId = categoryId,
                OwnerUserId = userId,
                Versions = new List<MaterialVersion>()
            };

            //creating new version dto
            var version = new MaterialVersion
            {
                FileSize = file.Length,
                FilePath = path,
                Material = material,
                VersionNumber = 1,
                CreatedAt = DateTime.Now
            };

            material.Versions.Add(version);

            //adding it to db
            await _dbContext.Materials.AddAsync(material);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Route("add_new_version")]
        public async Task<IActionResult> AddNewVersion([FromForm] IFormFile file, [FromForm] bool isActual=true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var material = _dbContext.Materials
                .FirstOrDefault(x => x.OwnerUserId == userId && x.Name == file.FileName);
            if (material == null)
                NotFound("File does not exist");

            var newVersionNum = _dbContext.MaterialVersions.Count(x => x.MaterialId == material.Id) + 1;

            //saving file
            var path = _fileManager.SaveFile(file, userId, newVersionNum).Result;

            //creating new version
            var newVersion = new MaterialVersion
            {
                FileSize = file.Length,
                FilePath = path,
                Material = material,
                VersionNumber = newVersionNum,
                CreatedAt = DateTime.Now
            };

            if (isActual)
                material.ActualVersionNum = newVersionNum;

            //saving changes to db
            await _dbContext.MaterialVersions.AddAsync(newVersion);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Route("download/{fileName}/{versionNum?}")]
        public async Task<IActionResult> DownloadFile(string fileName, int versionNum)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var material = _dbContext.Materials
                .Include(v => v.Versions)
                .FirstOrDefault(x => x.OwnerUserId == userId && x.Name == fileName);
            if (material == null)
                return NotFound("File does not exist");

            if (versionNum == 0)
                versionNum = material.ActualVersionNum;

            var materialVersion = material.Versions.FirstOrDefault(v => v.VersionNumber == versionNum);

            if (materialVersion == null)
                return NotFound("Version has not been found");

            var dataBytes = _fileManager.GetFileByName(userId, fileName, versionNum);
            
            return File(dataBytes, "application/octet-stream");
        }

        [HttpPost]
        [Route("edit_category")]
        public async Task<IActionResult> EditFileCategory([FromForm] string fileName, [FromForm] int newCategoryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var material = _dbContext.Materials
                .FirstOrDefault(x => x.OwnerUserId == userId && x.Name == fileName);
            if (material == null)
                return NotFound("File does not exist");

            if (newCategoryId < 1 || newCategoryId > 3)
                return BadRequest("Wrong category ID");

            if (material.CategoryId == newCategoryId)
                return Ok($"Category ID is already {newCategoryId}");

            var oldCategoryId = material.CategoryId;

            material.CategoryId = newCategoryId;

            await _dbContext.SaveChangesAsync();

            return Ok($"File's (\"{fileName}\") category ID changed from {oldCategoryId} to {newCategoryId}.");
        }

        [HttpGet]
        [Route("info/{fileName}")]
        public ActionResult MaterialInfo(string fileName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var material = _dbContext.Materials
                .Include(v => v.Versions)
                .FirstOrDefault(x => x.OwnerUserId == userId && x.Name == fileName);

            if (material == null)
                return NotFound("File does not exist");

            var materialVersion = material.Versions.FirstOrDefault(v => v.VersionNumber == material.ActualVersionNum);

            var output = Newtonsoft.Json.JsonConvert.SerializeObject(
                new
                {
                    material.Name,
                    category = ((MaterialCategories)material.CategoryId).ToString(),
                    material.ActualVersionNum,
                    materialVersion.FileSize
                });

            return Ok(output);
        }

        [HttpGet]
        [Route("filter_info")]
        public ActionResult FilteredInfo([FromForm] long minSize, [FromForm] int categoryId, [FromForm] long maxSize=-1)
        {
            var materials = _dbContext.Materials.Include(m => m.Versions).ToList();

            if (categoryId != 0)
            {
                if (categoryId < 1 || categoryId > 3)
                    return BadRequest("Wrong category ID");
                materials = materials.Where(x => x.CategoryId == categoryId).ToList();
            }

            if (maxSize != -1 || minSize != 0)
            {
                materials = materials
                    .Where(material =>
                        {
                            var versionNum = material.ActualVersionNum;
                            var version = material.Versions.FirstOrDefault(v => v.VersionNumber == versionNum);
                            return (version.FileSize >= minSize && (maxSize == -1 || version.FileSize <= maxSize));
                        }
                    ).ToList();
            }

            var output = Newtonsoft.Json.JsonConvert.SerializeObject(materials);
            return Ok(output);
        }
    }
}
