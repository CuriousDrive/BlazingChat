using BlazingChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlazingChat.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace BlazingChat.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<UserController> logger;
        private readonly BlazingChatContext _context;
        public UserController(ILogger<UserController> logger, BlazingChatContext context)
        {
            this.logger = logger;
            this._context = context;
        }

        //Authentication Methods
        [HttpPost("loginuser")]
        public async Task<ActionResult<User>> LoginUser(User user)
        {
            User loginUser = _context.Users.Where(u => u.EmailAddress == user.EmailAddress && u.Password == user.Password)
                                .ToList()
                                .FirstOrDefault();

            if (loginUser != null)
            {
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, loginUser.EmailAddress) });
                var claimsPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(claimsPrincipal);
            }

            return loginUser;
        }

        [HttpGet("logoutuser")]
        public async Task<IActionResult> LogOutUser()
        {
            await HttpContext.SignOutAsync();
            return Redirect("~/");
        }

        [HttpGet("getloggedinuser")]
        public async Task<ActionResult<User>> GetLoggedInUser()
        {
            User returnedUser = new User();
            
            if (User.Identity.IsAuthenticated)
            {
                var emailAddress = User.FindFirstValue(ClaimTypes.Email);
                returnedUser = _context.Users.Where(u => u.EmailAddress == emailAddress)
                                            .ToList()
                                            .FirstOrDefault();
            }

            return await Task.FromResult(returnedUser);
        }

        [HttpGet("getcontacts")]
        public List<User> GetContacts()
        {
            return _context.Users.ToList();
        }
        
        [HttpPut("updateprofile/{userId}")]
        public async Task<User> UpdateProfile(int userId, [FromBody] User user)
        {            
            User userToUpdate = await _context.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();
            
            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.EmailAddress = user.EmailAddress;
            userToUpdate.AboutMe = user.AboutMe;

            await _context.SaveChangesAsync();

            return await Task.FromResult(user);
        }

        [HttpGet("getprofile/{userId}")]
        public async Task<User> GetProfile(int userId)
        {
            return await _context.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();
        }

        [HttpGet("updatetheme")]
        public async Task<User> UpdateTheme(string userId, string value)
        {            
            User user = _context.Users.Where(u => u.UserId == Convert.ToInt32(userId)).FirstOrDefault();
            user.DarkTheme = value == "True" ? 1 : 0;

            await _context.SaveChangesAsync();

            return await Task.FromResult(user);
        }

        [HttpGet("updatenotifications")]
        public async Task<User> UpdateNotifications(string userId, string value)
        {
            User user = _context.Users.Where(u => u.UserId == Convert.ToInt32(userId)).FirstOrDefault();
            user.Notifications = value == "True" ? 1 : 0;

            await _context.SaveChangesAsync();

            return await Task.FromResult(user);
        }
    }
}
