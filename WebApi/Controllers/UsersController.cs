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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly UserDbContext _db;

        public UsersController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUserService userService, UserDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
            _db = db;
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
        public ActionResult Get()
        {
            return Ok(string.Join(Environment.NewLine,_userManager.Users.ToList()));
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Route("AddUserRole")]
        public async Task<IActionResult> AddUserRole([FromBody] string userName, string newRole)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound("User with userName given not found!");
            await _userManager.AddToRoleAsync(user, newRole);
            return Ok($"Added user role successfully.");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Route("RemoveUserRole")]
        public async Task<IActionResult> RemoveUserRole([FromBody] string userName, string newRole)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound("User with userName given not found!");
            await _userManager.RemoveFromRoleAsync(user, newRole);
            return Ok($"Removed user role successfully.");
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        [Route("{userName}")]
        public async Task<IActionResult> UserInfo(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return NotFound("User with userName given not found!");

            return Ok($"User ID : \"{user.Id}\"\n" +
                      $"User Login UserName : \"{user.UserName}\"\n" +
                      $"User Email : \"{user.Email}\"\n" +
                      $"User Name : \"{user.Name}\"\n" +
                      $"User Password Hash : \"{user.PasswordHash}\"\n");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Route("Edit")]
        public async Task<IActionResult> Edit([FromBody] ApplicationUser model)
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
            await SetInitialDataAsync();
            if (!ModelState.IsValid)
                return BadRequest("Invalid request body!");
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return BadRequest("User already exists!");

            var user = new ApplicationUser()
            {
                Email = model.Email,
                UserName = model.Username,
                Name = model.Name
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest($"User creation failed: {Newtonsoft.Json.JsonConvert.SerializeObject(result.Errors.ToList())}");

            await _userManager.AddToRoleAsync(user, "reader");

            await _signInManager.SignInAsync(user, false);
            
            return Ok("User created successfully!");
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            await SetInitialDataAsync();
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
            await _userManager.CreateAsync(new ApplicationUser
            {
                Email = "admin@gmail.com",
                UserName = "admin",
                Name = "Roman",
            });
            await _db.SaveChangesAsync();
            //await SetInitialDataAsync();
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
            await _userService.SetInitialData(new UserDTO
            {
                Email = "admin@gmail.com",
                UserName = "admin",
                Password = "admin",
                Name = "Roman",
                Role = "admin",
            }, new List<string> { "reader", "admin", "initiator" });
        }
    }
}
