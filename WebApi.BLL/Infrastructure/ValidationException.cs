using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.BLL.Infrastructure
{
    public class ValidationException : Exception
    {
        public string Property { get; private set; }
        public ValidationException(string message, string prop) : base(message)
        {
            Property = prop;
        }
    }
}
