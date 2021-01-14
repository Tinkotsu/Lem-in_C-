using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.BLL.BusinessModels.Material;
using WebApi.BLL.Infrastructure;
using WebApi.BLL.Interfaces;
using WebApi.DAL.Entities.Material;

namespace WebApi.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        public MaterialController(IMaterialService materialService) 
        {
            _materialService = materialService;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewFile([FromForm] IFormFile file, [FromForm] MaterialCategories category)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            byte[] fileBytes;

            await using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }
            
            var materialBm = new MaterialBm
            {
                Category = category,
                OwnerUserId = userId
            };

            var materialFileBm = new MaterialFileBm
            {
                FileName = file.FileName,
                FileSize = file.Length,
                FileBytes = fileBytes
            };
            
            _materialService.SaveMaterial(materialBm, materialFileBm);
            return Ok("Material has been added successfully");

        }

        [HttpPost]
        [Route("version")]
        public async Task <IActionResult> AddNewVersion([FromForm] IFormFile file, [FromForm] bool isActual=true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            
            byte[] fileBytes;

            await using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            var fileBm = new MaterialFileBm
            {
                FileBytes = fileBytes,
                FileName = file.FileName,
                FileSize = file.Length
            };

            _materialService.SaveMaterialVersion(fileBm, userId, isActual);
            return Ok("New material version added successfully.");
        }

        [HttpGet]
        [Route("{fileName}/{versionNum?}")]
        public ActionResult DownloadFile(string fileName, int? versionNum)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var materialFileBm = new MaterialBm
            {
                Name = fileName,
                ActualVersionNum = versionNum,
                OwnerUserId = userId
            };
            var dataBytes = _materialService.GetMaterialFile(materialFileBm);
            return File(dataBytes, "application/octet-stream", fileName);
        }

        [HttpPatch]
        [Route("category")]
        public ActionResult EditFileCategory([FromForm] string fileName, [FromForm] MaterialCategories newCategory)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var materialBm = new MaterialBm
            {
                Name = fileName,
                OwnerUserId = userId,
                Category = newCategory
            };
            
            _materialService.EditMaterialCategory(materialBm);
            return Ok($"File's (\"{fileName}\") category ID changed to {newCategory}.");
        }

        [HttpGet]
        [Route("info/{fileName}")]
        public ActionResult MaterialInfo(string fileName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var material = _materialService.GetMaterial(fileName, userId);

            var materialVersion = _materialService.GetMaterialVersion(material);

            var output = Newtonsoft.Json.JsonConvert.SerializeObject(
                new
                {
                    material.Name,
                    category = material.Category,
                    material.ActualVersionNum,
                    materialVersion.CreatedAt,
                    materialVersion.FileSize
                });

            return Ok(output);
        }

        [HttpGet]
        [Route("filter_info")]
        [Authorize(Roles = "admin")]
        public IActionResult FilteredInfo([FromBody] MaterialBm materialFilterRequest, long? minSize, long? maxSize)
        {
            var materials = _materialService.GetFilteredMaterials(materialFilterRequest, minSize, maxSize);

            var output = Newtonsoft.Json.JsonConvert.SerializeObject(materials.Select(material =>
                new
                {
                    material.Name,
                    material.Category,
                    material.ActualVersionNum
                }).ToList());

            return Ok(output);
        }
    }
}
