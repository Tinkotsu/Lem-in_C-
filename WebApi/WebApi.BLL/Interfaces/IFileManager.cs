using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApi.BLL.Interfaces
{
    public interface IFileManager
    {
        public Task<string> SaveFile(IFormFile file, string hash);
        public byte[] GetFile(string hash);
    }
}
