using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlazingChat.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace BlazingChat.Server.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly BlazingChatContext _context;

        public UserController(ILogger<UserController> logger, BlazingChatContext context)
        {
            this._logger = logger;
            this._context = context;
        }

        [HttpGet("user")]
        public Contacts Get()
        {
            var contact = new Contacts();

            if (User.Identity.IsAuthenticated)
            {
                contact.FirstName = User.Identity.Name;
            }

            return contact;
        }

        [HttpGet("user/getallusers")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            return await Task.FromResult(_context.User.ToList());
        }

        [HttpPost("user/register")]
        public async Task<ActionResult<User>> RegisterUser(User user)
        {

            if (_context.User.Count() > 0)
                user.UserId = _context.User.Max(u => u.UserId) + 1;
            else
                user.UserId = 1;

            user.Source = "APPL";
            _context.User.Add(user);

            await _context.SaveChangesAsync();

            return await Task.FromResult(user);
        }

        [HttpPost("user/login")]
        public async Task<ActionResult<User>> LoginUser(User user)
        {
            return _context.User.Where(u => u.EmailAddress == user.EmailAddress && u.Password == user.Password)
                                .ToList()
                                .FirstOrDefault();
        }

        [HttpGet("user/getprofile/{userId}")]
        public async Task<ActionResult<User>> GetProfile(int userId)
        {
            return _context.User.Where(u => u.UserId == userId).FirstOrDefault();
        }

        [HttpPut("user/updateprofile/{userId}")]
        public async Task<IActionResult> UpdateProfile(int userId, User user)
        {
            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("user/updatetheme")]
        public async Task<IActionResult> UpdateTheme([FromQuery] int userId, [FromQuery] bool value)
        {
            User user = _context.User.Where(u => u.UserId == userId).FirstOrDefault();
            user.DarkTheme = value? 1 : 0;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("user/updatenotifications")]
        public async Task<IActionResult> UpdateNotifications([FromQuery] int userId, [FromQuery] bool value)
        {
            User user = _context.User.Where(u => u.UserId == userId).FirstOrDefault();
            user.Notifications = value? 1 : 0;

            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("user/twittersignin")]
        public async Task TwitterSignIn(string redirectUri)
        {
            if (string.IsNullOrEmpty(redirectUri) || !Url.IsLocalUrl(redirectUri))
            {
                redirectUri = "/";
            }

            await HttpContext.ChallengeAsync(
                TwitterDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = redirectUri });
        }

        [HttpGet("user/googlesignin")]
        public async Task GoogleSignIn(string redirectUri)
        {
            if (string.IsNullOrEmpty(redirectUri) || !Url.IsLocalUrl(redirectUri))
            {
                redirectUri = "/";
            }

            await HttpContext.ChallengeAsync(
                GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = redirectUri });
        }

        [HttpGet("user/facebooksignin")]
        public async Task FacebookSignIn(string redirectUri)
        {
            if (string.IsNullOrEmpty(redirectUri) || !Url.IsLocalUrl(redirectUri))
            {
                redirectUri = "/";
            }

            await HttpContext.ChallengeAsync(
                FacebookDefaults.AuthenticationScheme,
                new AuthenticationProperties { RedirectUri = redirectUri });
        }


        [HttpGet("user/signout")]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("~/");
        }
    }
}
