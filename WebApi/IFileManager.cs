using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApi
{
    public interface IFileManager
    {
        public Task<string> SaveFile(IFormFile file, string userId, int versionNum);
        public byte[] GetFileByName(string userId, string fileName, int versionNum);
        public byte[] GetFileByPath(string filePath);
        public List<string> GetAllFiles();
    }
}
