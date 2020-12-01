using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Get()
        {
            return Ok(string.Join(Environment.NewLine,_userManager.Users.ToList()));
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Route("AddUserRole")]
        public async Task<IActionResult> AddUserRole([FromBody] ChangeUserRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound("User with userName given not found!");
            await _userManager.AddToRoleAsync(user, model.NewRole);
            return Ok($"Added user role successfully.");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Route("RemoveUserRole")]
        public async Task<IActionResult> RemoveUserRole([FromBody] ChangeUserRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return NotFound("User with userName given not found!");
            await _userManager.RemoveFromRoleAsync(user, model.NewRole);
            return Ok($"Removed user role successfully.");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Route("GetUserId")]
        public async Task<IActionResult> GetUserId([FromBody] string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound("User with userName given not found!");
            return Ok($"User ID : \"{user.Id}\"");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Route("Edit")]
        public async Task<IActionResult> Edit([FromBody] User model)
        {
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
                return NotFound("User not found!");
            user.Email = model.Email;
            user.Name = model.Name;
            user.UserName = model.UserName;
            return Ok("User is edited.");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Route("Delete")]
        public async Task<ActionResult> Delete([FromBody] string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found!");
            await _userManager.DeleteAsync(user);
            return Ok("User deleted");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request body!");
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return BadRequest("User already exists!");

            var user = new User()
            {
                Email = model.Email,
                UserName = model.Username,
                Name = model.Name
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            await _userManager.AddToRoleAsync(user, "reader");

            if (!result.Succeeded)
                return BadRequest("User creation failed!");

            await _signInManager.SignInAsync(user, false);
            
            return Ok("User created successfully!");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
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
            return Ok("Send post request with a model (username and password) to login.");
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            // deleting Cookie
            await _signInManager.SignOutAsync();
            return Ok("Logged out successfully.");
        }
    }
}
