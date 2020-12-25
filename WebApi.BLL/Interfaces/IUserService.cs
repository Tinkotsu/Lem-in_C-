using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.BLL.BusinessModels.User;

namespace WebApi.BLL.Interfaces
{
    public interface IUserService : IDisposable
    {
        Task Create(UserDTO userDTO);
        Task SetInitialData(UserDTO adminDTO, List<string> roles);
    }
}
