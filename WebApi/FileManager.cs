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
    public class FileManager
    {

        public async void AddNewFile(string userId, IFormFile file, int categoryId)
        {
        }

        public void AddNewVersion(string userId, IFormFile file, string fileName)
        {

        }

        public void DeleteFile(string userId, string fileName)
        {

        }

        public void DownVersion(string userId, string fileName, int destVersion)
        {

        }

        public string FileInfo(string userId, string fileName)
        {
            return "ok";
        }

        public byte[] GetFile(string userId, string fileName)
        {
            return new byte['1'];
        }

        public byte[] GetFileVersion(string userId, string fileName, int version)
        {
            return new byte['1'];
        }

        public void ChangeFileCategory(string userId, string fileName, int newCategoryId)
        {

        }
    }
}
