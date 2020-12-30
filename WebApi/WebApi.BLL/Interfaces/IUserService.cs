using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApi.BLL.BusinessModels.User;

namespace WebApi.BLL.Interfaces
{
    public interface IUserService // : IDisposable
    {
        Task Create(UserBM userDTO);

        Task SetInitialData(UserBM adminDTO, List<string> roles);

        public IEnumerable<UserBM> GetAllUsers();

        public Task<UserBM> GetUser(string userName);

        public Task AddUserRole(string userName, string newRole);

        public Task RemoveUserRole(string userName, string role);

        public Task DeleteUser(string userName);
    }
}
