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
        
        private readonly MaterialDbContext _dbContext;
        private readonly IFileManager _fileManager;
        private string _userId;

        public MaterialController(MaterialDbContext dbContext, IFileManager fileManager, string userId=null) // ? userId ? 
        {
            _userId = userId;
            _dbContext = dbContext;
            _fileManager = fileManager;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewFile([FromForm] IFormFile file, [FromForm] int categoryId)
        {
            _userId ??= User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (file == null)
                return BadRequest("No file uploaded");
            if (categoryId < 1 || categoryId > 3)
                return BadRequest("Wrong category ID");
            if (_dbContext.Materials.Any())
            {
                if (_dbContext.Materials
                    .Count(dbFile => dbFile.OwnerUserId == _userId && dbFile.Name == file.FileName) != 0)
                    return BadRequest("File already exist");
            }

            //saving file
            var path = _fileManager.SaveFile(file, _userId, 1).Result;

            //creating new material dto
            var material = new Material
            {
                Name = file.FileName,
                ActualVersionNum = 1,
                CategoryId = categoryId,
                OwnerUserId = _userId,
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

            return Ok("File created successfully.");
        }

        [HttpPost]
        [Route("version")]
        public async Task<IActionResult> AddNewVersion([FromForm] IFormFile file, [FromForm] bool isActual=true)
        {
            _userId ??= User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var material = _dbContext.Materials
                .FirstOrDefault(x => x.OwnerUserId == _userId && x.Name == file.FileName);
            
            if (material == null)
                return NotFound("File does not exist");

            var newVersionNum = _dbContext.MaterialVersions.Count(x => x.MaterialId == material.Id) + 1;

            //saving file
            var path = _fileManager.SaveFile(file, _userId, newVersionNum).Result;

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

            return Ok("New version added successfully.");
        }

        [HttpGet]
        [Route("{fileName}/{versionNum?}")]
        public async Task<IActionResult> DownloadFile(string fileName, int versionNum=0)
        {
            _userId ??= User.FindFirstValue(ClaimTypes.NameIdentifier);
            var material = _dbContext.Materials
                .Include(v => v.Versions)
                .FirstOrDefault(x => x.OwnerUserId == _userId && x.Name == fileName);
            if (material == null)
                return NotFound("File does not exist");

            if (versionNum == 0)
                versionNum = material.ActualVersionNum;

            var materialVersion = material.Versions.FirstOrDefault(v => v.VersionNumber == versionNum);

            if (materialVersion == null)
                return NotFound("Version has not been found");

            var dataBytes = _fileManager.GetFileByName(_userId, fileName, versionNum);
            
            return File(dataBytes, "application/octet-stream", fileName);
        }

        [HttpPatch]
        [Route("category")]
        public async Task<IActionResult> EditFileCategory([FromForm] string fileName, [FromForm] int newCategoryId)
        {
            _userId ??= User.FindFirstValue(ClaimTypes.NameIdentifier);
            var material = _dbContext.Materials
                .FirstOrDefault(x => x.OwnerUserId == _userId && x.Name == fileName);
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
            _userId ??= User.FindFirstValue(ClaimTypes.NameIdentifier);
            var material = _dbContext.Materials
                .Include(v => v.Versions)
                .FirstOrDefault(x => x.OwnerUserId == _userId && x.Name == fileName);

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
        [Authorize(Roles = "admin")]
        public IActionResult FilteredInfo([FromForm] int categoryId, [FromForm] long? minSize=null, [FromForm] long? maxSize=null)
        {
            var materials = _dbContext.Materials.Include(m => m.Versions).ToList();

            if (minSize != null && maxSize != null)
            {
                if (maxSize < minSize || maxSize < 0 || minSize < 0)
                    return BadRequest("Wrong min max size");
            }

            minSize ??= 0;
            maxSize ??= -1;

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
                        })
                    .ToList();
            }

            var output = Newtonsoft.Json.JsonConvert.SerializeObject(materials.Select(material =>
                new
                {
                    material.Name,
                    material.CategoryId,
                    material.ActualVersionNum,
                    versionsCount = material.Versions.Count
                }).ToList());

            return Ok(output);
        }

        public List<string> GetAllLocalMaterialsList()
        {
            return _fileManager.GetAllFiles();
        }
    }
}
