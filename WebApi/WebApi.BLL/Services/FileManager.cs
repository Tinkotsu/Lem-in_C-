using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using WebApi.BLL.Interfaces;

namespace WebApi.BLL.Services
{
    public class FileManager : IFileManager
    {
        private string DirName { get; set; }

        public FileManager(IConfiguration configuration)
        {
            DirName = configuration["FilesFolder"];
            Directory.CreateDirectory(DirName);
        }

        public async Task<string> SaveFile(byte[] fileBytes, string hash)
        { 
            var path = DirName + '/' + hash;

            await File.WriteAllBytesAsync(path, fileBytes);

            return hash;
        }

        public byte[] GetFile(string hash)
        {
            var path = DirName + '/' + hash;

            var file = File.ReadAllBytesAsync(path);

            return file.Result;
        }
    }
}
