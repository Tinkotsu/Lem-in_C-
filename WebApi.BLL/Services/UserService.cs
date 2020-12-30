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

        public async Task Create(UserBM userBM)
        {
            if (userBM.Email == null || userBM.Password == null || userBM.Name == null)
                throw new ValidationException("Wrong register model", "");

            var user = await _unitOfWork.UserManager.FindByEmailAsync(userBM.Email);
            if (user != null)
                throw new ValidationException("User with given email already exists", "Email");
            user = await _unitOfWork.UserManager.FindByNameAsync(userBM.UserName);
            if (user != null)
                throw new ValidationException("User with given user name already exists", "userName");

            user = new ApplicationUser { Email = userBM.Email, UserName = userBM.UserName, Name = userBM.Name };
            await _unitOfWork.UserManager.CreateAsync(user, userBM.Password);

            // adding role to user
            await _unitOfWork.UserManager.AddToRoleAsync(user, userBM.Role);
            await _unitOfWork.SaveAsync();
        }

        public async Task<UserBM> GetUser(string userName)
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ApplicationUser, UserBM>()).CreateMapper();

            var user = await _unitOfWork.UserManager.FindByNameAsync(userName);

            if (user == null)
                throw new ValidationException("User not found", "userName");

            return mapper.Map<ApplicationUser, UserBM>(user);
        }

        public IEnumerable<UserBM> GetAllUsers()
        {
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ApplicationUser, UserBM>()).CreateMapper();
            return mapper.Map<IEnumerable<ApplicationUser>, List<UserBM>>(_unitOfWork.UserManager.Users);
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

        public async Task SetInitialData(UserBM adminBM, List<string> roles)
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
            var user = await _unitOfWork.UserManager.FindByNameAsync(adminBM.UserName);
            if (user == null)
                await Create(adminBM);
        }

        //public void Dispose()
        //{
        //    _unitOfWork.Dispose();
        //}
    }
}
