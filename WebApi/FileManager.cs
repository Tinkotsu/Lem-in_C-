using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApi.AppData;
using WebApi.Models;

namespace WebApi
{
    public class FileManager : IFileManager
    {
        private const string DirName = "Files";
        public async Task<string> SaveFile(IFormFile file, string userId, int versionNum)
        {
            var path = string.Join('/', DirName, userId, file.FileName, versionNum.ToString());
            Directory.CreateDirectory(path);
            path += "/" + file.FileName;
            await using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return path;
        }

        public byte[] GetFileByPath(string filePath)
        {
            var file = System.IO.File.ReadAllBytesAsync(filePath);

            return file.Result;
        }

        public byte[] GetFileByName(string userId, string fileName, int versionNum)
        {
            var filePath = string.Join('/', DirName, userId, fileName, versionNum.ToString(), fileName);

            return GetFileByPath(filePath);
        }
    }
}
