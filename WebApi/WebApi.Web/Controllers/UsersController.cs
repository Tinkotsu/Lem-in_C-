using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Extensions.Configuration;
using WebApi.BLL.BusinessModels.User;
using WebApi.BLL.Infrastructure;
using WebApi.BLL.Interfaces;
using WebApi.DAL.EF;
using WebApi.DAL.Entities.User;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;

        public UsersController(SignInManager<ApplicationUser> signInManager, IUserService userService)
        {
            _userService = userService;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("forbidden")]
        public ActionResult Forbidden()
        {
            return StatusCode(403, "Access denied!");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        [Route("{userName?}")]
        public async Task<ActionResult> GetAsync(string userName)
        {
            string output;
            if (userName != null)
            {
                var user = await _userService.GetUser(userName);
                output = Newtonsoft.Json.JsonConvert.SerializeObject(user);
            }
            else
            {
                var users = _userService.GetAllUsers();
                output = Newtonsoft.Json.JsonConvert.SerializeObject(users);
            }
            return Ok(output);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Route("role")]
        public async Task<IActionResult> AddUserRole([FromBody] string userName, string newRole)
        {
            await _userService.AddUserRole(userName, newRole);
            return Ok($"Added user role successfully.");
        }

        [HttpDelete]
        [Authorize(Roles = "admin")]
        [Route("role")]
        public async Task<IActionResult> RemoveUserRole([FromBody] string userName, string role)
        {
            await _userService.RemoveUserRole(userName, role);
            return Ok($"Removed user role successfully.");
        }

        [HttpDelete]
        [Authorize(Roles = "admin")]
        [Route("userName")]
        public async Task<ActionResult> Delete([FromQuery] string userName)
        {
            await _userService.DeleteUser(userName);
            return Ok("User deleted");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            await _userService.Create(new UserBm
            {
                Email = model.Email,
                UserName = model.Username,
                Name = model.Name,
                Password = model.Password,
                Role = "reader"
            });

            return Ok("User created");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            //await SetInitialDataAsync();
            if (User.Identity.IsAuthenticated)
                return BadRequest("User is already authenticated. Logout to change user.");

            if (!ModelState.IsValid)
                return BadRequest("Wrong login input!");

            var result = await _signInManager.PasswordSignInAsync(
                model.UserName, model.Password, model.RememberMe, false);
            
            if (!result.Succeeded)
                return BadRequest("Wrong login and/or password!");

            return Ok("Logged in successfully.");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login()
        {
            await SetInitialDataAsync();

            if (User.Identity.IsAuthenticated)
                return BadRequest("User is already authenticated. Logout to change user.");

            return Unauthorized("Unauthorized user. Send post request with a model (username and password) to login.");
        }

        [HttpGet]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            // deleting Cookie
            await _signInManager.SignOutAsync();
            return Ok("Logged out successfully.");
        }

        private async Task SetInitialDataAsync()
        {
            await _userService.SetInitialData(new UserBm
            {
                Email = "admin2@gmail.com",
                UserName = "admin",
                Password = "admin12",
                Name = "Roman",
                Role = "admin"
            }, new List<string> { "reader", "admin", "initiator" });
        }
    }
}
