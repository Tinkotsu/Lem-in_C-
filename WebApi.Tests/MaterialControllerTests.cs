//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.HttpOverrides;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using WebApi.Controllers;
//using Xunit;

//namespace WebApi.Tests
//{
//    public class MaterialControllerTests
//    {
//        private const string UserId = "d3c1f926-8c5f-4eb3-aeed-5c6ae2325331";

//        private readonly MaterialController _controller = new MaterialController(new MaterialDbContext
//        (new DbContextOptionsBuilder<MaterialDbContext>()
//            .UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SenatTestAPIDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False")
//            .Options), new FileManager(), UserId);


//        [Fact]
//        public void AddNewFile_ValidFileShouldBeOk()
//        {
//            const string fileName = "test_file.txt";
//            const string path = @"C:\Users\Roman\source\repos\.NET\WebApi.Tests\test_files\" + fileName;

//            var oldMaterialsCount = _controller.GetAllLocalMaterialsList().Count;

//            OkObjectResult result = null;

//            using var stream = File.OpenRead(path);
//            {
//                var formFile = new FormFile(stream, 0, stream.Length, null, fileName)
//                {
//                    Headers = new HeaderDictionary(),
//                    ContentType = "text/plain"
//                };
//                //act
//                result = (OkObjectResult)_controller.AddNewFile(formFile, 1).Result;
//            }

//            var newMaterialsList = _controller.GetAllLocalMaterialsList();

//            //assert
//            Assert.Equal(200, result.StatusCode);
//            Assert.Equal(oldMaterialsCount + 1, newMaterialsList.Count);
//            Assert.Equal(fileName, newMaterialsList.Last());
//        }

//        [Fact]
//        public void AddNewFile_NoFileShouldBeBadRequest()
//        {
//            //act
//            var result = (BadRequestObjectResult)_controller.AddNewFile(null, 1).Result;

//            //assert
//            Assert.Equal(400, result.StatusCode);
//        }

//        [Fact]
//        public void AddNewFile_WrongCategoryShouldBeBadRequest()
//        {
//            const string fileName = "test_file.txt";
//            const string path = @"C:\Users\Roman\source\repos\.NET\WebApi.Tests\test_files\" + fileName;

//            BadRequestObjectResult result = null;

//            using var stream = File.OpenRead(path);
//            {
//                var formFile = new FormFile(stream, 0, stream.Length, null, fileName)
//                {
//                    Headers = new HeaderDictionary(),
//                    ContentType = "text/plain"
//                };
//                //act
//                result = (BadRequestObjectResult)_controller.AddNewFile(formFile, 4).Result;
//            }

//            var newMaterialsList = _controller.GetAllLocalMaterialsList();

//            //assert
//            Assert.Equal(400, result.StatusCode);
//        }

//        [Fact]
//        public void AddNewFile_FileDuplicateShouldBeBadRequest()
//        {
//            const string fileName = "test_file.txt";
//            const string path = @"C:\Users\Roman\source\repos\.NET\WebApi.Tests\test_files\" + fileName;

//            BadRequestObjectResult result = null;

//            //should be doubled in mock version of the test
//            using var stream = File.OpenRead(path);
//            {
//                var formFile = new FormFile(stream, 0, stream.Length, null, fileName)
//                {
//                    Headers = new HeaderDictionary(),
//                    ContentType = "text/plain"
//                };
//                //act
//                var firstResult = _controller.AddNewFile(formFile, 1).Result;
//                result = (BadRequestObjectResult)_controller.AddNewFile(formFile, 1).Result;
//            }

//            var newMaterialsList = _controller.GetAllLocalMaterialsList();

//            //assert
//            Assert.Equal(400, result.StatusCode);
//        }

//        [Fact]
//        public void AddNewVersion_ValidFileShouldBeOk()
//        {
//            const string fileName = "test_file.txt";
//            const string path = @"C:\Users\Roman\source\repos\.NET\WebApi.Tests\test_files\" + fileName;

//            var oldMaterialsCount = _controller.GetAllLocalMaterialsList().Count;

//            OkObjectResult result = null;

//            using var stream = File.OpenRead(path);
//            {
//                var formFile = new FormFile(stream, 0, stream.Length, null, fileName)
//                {
//                    Headers = new HeaderDictionary(),
//                    ContentType = "text/plain"
//                };
//                //act
//                result = (OkObjectResult)_controller.AddNewVersion(formFile).Result;
//            }

//            var newMaterialsList = _controller.GetAllLocalMaterialsList();

//            //assert
//            Assert.Equal(200, result.StatusCode);
//            Assert.Equal(oldMaterialsCount + 1, newMaterialsList.Count);
//            Assert.Equal(fileName, newMaterialsList.Last());
//        }

//        [Fact]
//        public void AddNewVersion_ValidFileShouldBeNotFound()
//        {
//            const string fileName = "not_found_test_file.txt";
//            const string path = @"C:\Users\Roman\source\repos\.NET\WebApi.Tests\test_files\" + fileName;

//            NotFoundObjectResult result = null;

//            using var stream = File.OpenRead(path);
//            {
//                var formFile = new FormFile(stream, 0, stream.Length, null, fileName)
//                {
//                    Headers = new HeaderDictionary(),
//                    ContentType = "text/plain"
//                };
//                //act
//                result = (NotFoundObjectResult)_controller.AddNewVersion(formFile).Result;
//            }

//            //assert
//            Assert.Equal(404, result.StatusCode);
//        }

//        [Fact]
//        public void MaterialInfo_ValidFileNameShouldBeOkay()
//        {
//            const string fileName = "test_file.txt";

//            //act
//            var result = (OkObjectResult)_controller.MaterialInfo(fileName);

//            //assert
//            Assert.True(result?.Value?.ToString()?.Length > 0);
//            Assert.Equal(200, result?.StatusCode);
//        }

//        [Fact]
//        public void MaterialInfo_InvalidFileNameShouldBeNotFound()
//        {
//            //act
//            var result = (NotFoundObjectResult)_controller.MaterialInfo("not_found_file.file");

//            //assert
//            Assert.Equal(404, result.StatusCode);
//        }

//        [Fact]
//        public void DownloadFile_ValidFileNameActualVersionShouldBeFileContentResult()
//        {
//            const string fileName = "test_file.txt";

//            //act
//            var result = (FileContentResult)_controller.DownloadFile(fileName).Result;

//            //assert
//            Assert.Equal(fileName, result.FileDownloadName);
//        }

//        [Fact]
//        public void DownloadFile_ValidFileNameVersionTwoShouldBeFileContentResult()
//        {
//            const string fileName = "test_file.txt";

//            //act
//            var result = (FileContentResult)_controller.DownloadFile(fileName, 2).Result;

//            //assert
//            Assert.Equal(fileName, result.FileDownloadName);
//        }

//        [Fact]
//        public void DownloadFile_InvalidFileNameShouldBeNotFound()
//        {
//            const string fileName = "not_found";

//            //act
//            var result = (NotFoundObjectResult)_controller.DownloadFile(fileName).Result;

//            //assert
//            Assert.Equal(404, result.StatusCode);
//        }

//        [Fact]
//        public void DownloadFile_InvalidVersionShouldBeNotFound()
//        {
//            const string fileName = "test_file.txt";

//            //act
//            var result = (NotFoundObjectResult)_controller.DownloadFile(fileName, 9999).Result;

//            //assert
//            Assert.Equal(404, result.StatusCode);
//        }

//        [Fact]
//        public void EditFileCategory_ValidFileValidCategoryShouldBeOk()
//        {
//            const string fileName = "test_file.txt";
//            const int newCategoryId = 2;

//            //act
//            var result = (OkObjectResult)_controller.EditFileCategory(fileName, newCategoryId).Result;

//            //assert
//            Assert.Equal(200, result.StatusCode);
//        }

//        [Fact]
//        public void EditFileCategory_InvalidFileValidCategoryShouldBeNotFound()
//        {
//            const string fileName = "test_file_invalid.txt";
//            const int newCategoryId = 2;

//            //act
//            var result = (NotFoundObjectResult)_controller.EditFileCategory(fileName, newCategoryId).Result;

//            //assert
//            Assert.Equal(404, result.StatusCode);
//        }

//        [Fact]
//        public void EditFileCategory_ValidFileInvalidCategoryShouldBeBadRequest()
//        {
//            const string fileName = "test_file.txt";
//            const int newCategoryId = 999;

//            //act
//            var result = (BadRequestObjectResult)_controller.EditFileCategory(fileName, newCategoryId).Result;

//            //assert
//            Assert.Equal(400, result.StatusCode);
//        }

//        [Fact]
//        public void FilteredInfo_ValidShouldBeOk()
//        {
//            const int categoryId = 1;
//            const long minSize = 0;
//            const long maxSize = 100;

//            //act
//            var result = (OkObjectResult) _controller.FilteredInfo(categoryId, minSize, maxSize);

//            //assert
//            Assert.Equal(200, result.StatusCode);
//            Assert.True(result?.Value?.ToString()?.Length > 0);
//        }

//        [Fact]
//        public void FilteredInfo_InvalidMinMaxSizeShouldBeBadRequest()
//        {
//            const int categoryId = 1;
//            const long minSize = -1;
//            const long maxSize = -1;

//            //act
//            var result = (BadRequestObjectResult)_controller.FilteredInfo(categoryId, minSize, maxSize);

//            //assert
//            Assert.Equal(400, result.StatusCode);
//        }

//        [Fact]
//        public void FilteredInfo_InvalidCategoryShouldBeBadRequest()
//        {
//            const int categoryId = 999;
//            const long minSize = 1;
//            const long maxSize = 100;

//            //act
//            var result = (BadRequestObjectResult)_controller.FilteredInfo(categoryId, minSize, maxSize);

//            //assert
//            Assert.Equal(400, result.StatusCode);
//        }

//    }
//}
