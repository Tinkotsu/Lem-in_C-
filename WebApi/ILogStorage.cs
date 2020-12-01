using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi
{
    public interface ILogStorage
    {
        public void Store(LogModel log);
    }
}
