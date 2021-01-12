using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace WebApi.BLL.Services
{
    public static class HashCalculator
    {
        public static string CalculateMd5(byte[] fileBytes)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(fileBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
