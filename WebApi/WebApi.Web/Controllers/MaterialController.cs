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
using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.BLL.Interfaces;
using WebApi.BLL.BusinessModels.Material;
using WebApi.BLL.Infrastructure;
using WebApi.DAL.Entities.Material;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialController : ControllerBase
    {
        IMaterialService _materialService;

        public MaterialController(IMaterialService materialService) 
        {
            _materialService = materialService;
        }

        [HttpPost]
        public ActionResult AddNewFile([FromForm] IFormFile file, [FromForm] int categoryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var newSaveMaterialBM = new SaveMaterialBM {
                File = file,
                CategoryId = categoryId,
                UserId = userId
            };

            try
            {
                _materialService.SaveMaterial(newSaveMaterialBM);
                return Ok("Material has been added successfully");
            }
            catch (ValidationException ex)
            {
                return BadRequest($"Error: {ex.Message} ({ex.Property})");
            }

        }

        [HttpPost]
        [Route("version")]
        public ActionResult AddNewVersion([FromForm] IFormFile file, [FromForm] bool isActual=true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var newSaveMaterialVersionBM = new SaveMaterialVersionBM
            {
                File = file,
                UserId = userId,
                IsActual = isActual
            };

            try
            {
                _materialService.SaveMaterialVersion(newSaveMaterialVersionBM);
                return Ok("New material version added successfully.");
            }
            catch (ValidationException ex)
            {
                return BadRequest($"Error: {ex.Message} ({ex.Property})");
            }
            
        }

        [HttpGet]
        [Route("{fileName}/{versionNum?}")]
        public ActionResult DownloadFile(string fileName, int? versionNum)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var getMaterialFileBM = new GetMaterialFileBM
                {
                    FileName = fileName,
                    VersionNumber = versionNum,
                    UserId = userId
                };
                var dataBytes = _materialService.GetMaterialFile(getMaterialFileBM);
                return File(dataBytes, "application/octet-stream", fileName);
            }
            catch (ValidationException ex)
            {
                return BadRequest($"Error {ex.Message}");
            }
            
        }

        [HttpPatch]
        [Route("category")]
        public ActionResult EditFileCategory([FromForm] string fileName, [FromForm] int newCategoryId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var editCategoryBM = new EditCategoryBM
                {
                    FileName = fileName,
                    NewCategoryID = newCategoryId,
                    UserId = userId
                };

                _materialService.EditMaterialCategory(editCategoryBM);

                return Ok($"File's (\"{fileName}\") category ID changed to {newCategoryId}.");
            }
            catch (ValidationException ex)
            {
                return BadRequest($"Error {ex.Message}");
            }

        }

        [HttpGet]
        [Route("info/{fileName}")]
        public ActionResult MaterialInfo(string fileName)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var material = _materialService.GetMaterial(fileName, userId);

                var materialVersion = _materialService.GetMaterialVersion(material);

                var output = Newtonsoft.Json.JsonConvert.SerializeObject(
                    new
                    {
                        material.Name,
                        category = ((MaterialCategories)material.CategoryId).ToString(),
                        material.ActualVersionNum,
                        materialVersion.CreatedAt,
                        materialVersion.FileSize
                    });

                return Ok(output);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpGet]
        [Route("filter_info")]
        [Authorize(Roles = "admin")]
        public IActionResult FilteredInfo([FromBody] MaterialsFilterRequestBM materialFilterRequest)
        {
            try
            {
                var materials = _materialService.GetFilteredMaterials(materialFilterRequest);

                var output = Newtonsoft.Json.JsonConvert.SerializeObject(materials.Select(material =>
                    new
                    {
                        material.Name,
                        material.CategoryId,
                        material.ActualVersionNum
                    }).ToList());

                return Ok(output);
            }
            catch (ValidationException ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
