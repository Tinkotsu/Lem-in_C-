using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApi.BLL.BusinessModels.User;
using WebApi.BLL.Infrastructure;
using WebApi.BLL.Interfaces;
using WebApi.DAL.Entities.User;
using WebApi.DAL.Repositories;

namespace WebApi.BLL.Services
{
    public class UserService : IUserService
    {

        private readonly IIdentityUnitOfWork _unitOfWork;

        public UserService(IIdentityUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Create(UserDTO userDto)
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(userDto.Email);
            if (user == null)
            {
                user = new ApplicationUser { Email = userDto.Email, UserName = userDto.Email, Name = userDto.Name };
                await _unitOfWork.UserManager.CreateAsync(user, userDto.Password);

                // adding role to user
                await _unitOfWork.UserManager.AddToRoleAsync(user, userDto.Role);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task SetInitialData(UserDTO adminDto, List<string> roles)
        {
            foreach (string roleName in roles)
            {
                var role = await _unitOfWork.RoleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    role = new IdentityRole { Name = roleName };
                    await _unitOfWork.RoleManager.CreateAsync(role);
                }
            }
            await Create(adminDto);
        }

        //public void Dispose()
        //{
        //    _unitOfWork.Dispose();
        //}
    }
}
