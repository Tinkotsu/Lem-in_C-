using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
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
            if (userDto.Email == null || userDto.Password == null || userDto.Name == null)
                throw new ValidationException("Wrong register model", "");

            var user = await _unitOfWork.UserManager.FindByEmailAsync(userDto.Email);
            if (user != null)
                throw new ValidationException("User with given email already exists", "Email");
            user = await _unitOfWork.UserManager.FindByNameAsync(userDto.UserName);
            if (user != null)
                throw new ValidationException("User with given user name already exists", "userName");

            user = new ApplicationUser { Email = userDto.Email, UserName = userDto.UserName, Name = userDto.Name };
            await _unitOfWork.UserManager.CreateAsync(user, userDto.Password);

            // adding role to user
            await _unitOfWork.UserManager.AddToRoleAsync(user, userDto.Role);
            await _unitOfWork.SaveAsync();
        }

        public async Task<UserDTO> GetUser(string userName)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ApplicationUser, UserDTO>()).CreateMapper();

            var user = await _unitOfWork.UserManager.FindByNameAsync(userName);

            if (user == null)
                throw new ValidationException("User not found", "userName");

            return mapper.Map<ApplicationUser, UserDTO>(user);
        }

        public IEnumerable<UserDTO> GetAllUsers()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ApplicationUser, UserDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<ApplicationUser>, List<UserDTO>>(_unitOfWork.UserManager.Users);
        }

        public async Task AddUserRole(string userName, string newRole)
        {
            var user = await _unitOfWork.UserManager.FindByNameAsync(userName);

            if (user == null)
                throw new ValidationException("User not found", userName);

            await _unitOfWork.UserManager.AddToRoleAsync(user, newRole);
        }

        public async Task RemoveUserRole(string userName, string role)
        {
            var user = await _unitOfWork.UserManager.FindByNameAsync(userName);

            if (user == null)
                throw new ValidationException("User not found", userName);

            await _unitOfWork.UserManager.RemoveFromRoleAsync(user, role);
        }

        public async Task DeleteUser(string userName)
        {
            var user = await _unitOfWork.UserManager.FindByNameAsync(userName);

            if (user == null)
                throw new ValidationException("User not found", userName);

            await _unitOfWork.UserManager.DeleteAsync(user);
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
            var user = await _unitOfWork.UserManager.FindByNameAsync(adminDto.UserName);
            if (user == null)
                await Create(adminDto);
        }

        //public void Dispose()
        //{
        //    _unitOfWork.Dispose();
        //}
    }
}
