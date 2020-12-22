using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace WebApi.BLL.DTO.Material
{
    public class FileDTO
    {
        public IFormFile FormFile { get; set; }
        public int CategoryId { get; set; }
    }
}
