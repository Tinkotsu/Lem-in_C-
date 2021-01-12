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
using WebApi.DAL.Interfaces;
using WebApi.DAL.Repositories;

namespace WebApi.BLL.Services
{
    public class UserService : IUserService
    {

        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Create(UserBm userBm)
        {
            if (userBm.Email == null || userBm.Password == null || userBm.Name == null)
                throw new ValidationException("Wrong register model", "");

            var user = await _unitOfWork.UserManager.FindByEmailAsync(userBm.Email);
            if (user != null)
                throw new ValidationException("User with given email already exists", "Email");
            user = await _unitOfWork.UserManager.FindByNameAsync(userBm.UserName);
            if (user != null)
                throw new ValidationException("User with given user name already exists", "userName");

            user = new ApplicationUser { Email = userBm.Email, UserName = userBm.UserName, Name = userBm.Name };
            await _unitOfWork.UserManager.CreateAsync(user, userBm.Password);

            // adding role to user
            await _unitOfWork.UserManager.AddToRoleAsync(user, userBm.Role);
            await _unitOfWork.SaveAsync();
        }

        public async Task<UserBm> GetUser(string userName)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ApplicationUser, UserBm>()).CreateMapper();

            var user = await _unitOfWork.UserManager.FindByNameAsync(userName);

            if (user == null)
                throw new ValidationException("User not found", "userName");

            return mapper.Map<ApplicationUser, UserBm>(user);
        }

        public IEnumerable<UserBm> GetAllUsers()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ApplicationUser, UserBm>()).CreateMapper();
            return mapper.Map<IEnumerable<ApplicationUser>, List<UserBm>>(_unitOfWork.UserManager.Users);
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

        public async Task SetInitialData(UserBm adminBm, List<string> roles)
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
            var user = await _unitOfWork.UserManager.FindByNameAsync(adminBm.UserName);
            if (user == null)
                await Create(adminBm);
        }

        //public void Dispose()
        //{
        //    _unitOfWork.Dispose();
        //}
    }
}
