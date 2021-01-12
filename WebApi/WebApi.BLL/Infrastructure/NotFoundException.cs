using System;

namespace WebApi.BLL.Infrastructure
{
    public class NotFoundException : Exception
    {
        public string Property { get; private set; }
        public NotFoundException(string message, string prop) : base(message)
        {
            Property = prop;
        }
    }
}