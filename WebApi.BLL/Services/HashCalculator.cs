using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace WebApi.BLL.Services
{
    public class HashCalculator
    {
        public static string CalculateMD5(IFormFile file)
        {
            byte[] fileBytes = null;

            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(fileBytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
