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
using System.Security.Claims;

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
        public async Task<User> Get()
        {
            User user = new User();
            User returnedUser = new User();

            if (User.Identity.IsAuthenticated)
            {
                var emailAddress = User.FindFirstValue(ClaimTypes.Email);
                returnedUser = _context.User.Where(u => u.EmailAddress == emailAddress)
                                            .ToList()
                                            .FirstOrDefault();

                //case 1: Application User logging in
                //case 2: Application User Registering
                //case 3: External User Logging in 
                if (returnedUser != null && (returnedUser.Source == "APPL" || returnedUser.Source == "EXTL"))
                {
                    return returnedUser;
                }
                //case 4: External User Registering
                else
                {
                    user.EmailAddress = User.FindFirstValue(ClaimTypes.Email);
                    user.Password = user.EmailAddress;
                    user.Source = "EXTL";
                    user.Notifications = 0;
                    user.DarkTheme = 0;

                    if (_context.User.Count() > 0)
                        user.UserId = _context.User.Max(u => u.UserId) + 1;
                    else
                        user.UserId = 1;

                    _context.User.Add(user);

                    await _context.SaveChangesAsync();
                    return user;
                }
            }
            //case 5: User is not logged in
            else
                return returnedUser;
        }

        [HttpGet("claimsprincipal")]
        public string ClaimsPrincipal()
        {
            return User.FindFirstValue(ClaimTypes.Email);
        }

        [HttpPost("user/register")]
        public async Task<ActionResult<User>> RegisterUser(User user)
        {
            if (_context.User.Count() > 0)
                user.UserId = _context.User.Max(u => u.UserId) + 1;
            else
                user.UserId = 1;

            user.Notifications = 0;
            user.DarkTheme = 0;
            user.Source = "APPL";
            _context.User.Add(user);

            await _context.SaveChangesAsync();

            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, user.EmailAddress) }, "serverauth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(claimsPrincipal);

            return await Task.FromResult(user);
        }

        [HttpPost("user/login")]
        public async Task<ActionResult<User>> LoginUser(User user)
        {
            User returnUser = _context.User.Where(u => u.EmailAddress == user.EmailAddress && u.Password == user.Password)
                                .ToList()
                                .FirstOrDefault();

            if (returnUser != null)
            {
                var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, returnUser.EmailAddress) }, "serverauth");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(claimsPrincipal);
            }

            return returnUser;
        }

        [HttpGet("user/getallusers")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            return await Task.FromResult(_context.User.ToList());
        }


        [HttpGet("user/getprofile/{userId}")]
        public async Task<ActionResult<User>> GetProfile(int userId)
        {
            return await _context.User.Where(u => u.UserId == userId).FirstOrDefaultAsync();
        }

        [HttpPut("user/updateprofile/{userId}")]
        public async Task<IActionResult> UpdateProfile(int userId, User user)
        {
            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("user/updatetheme")]
        public async Task<User> UpdateTheme(string userId, string value)
        {
            User user = _context.User.Where(u => u.UserId == Convert.ToInt32(userId)).FirstOrDefault();
            user.DarkTheme = value == "True" ? 1 : 0;

            await _context.SaveChangesAsync();

            return await Task.FromResult(user);
        }

        [HttpGet("user/updatenotifications")]
        public async Task<User> UpdateNotifications(string userId, string value)
        {
            User user = _context.User.Where(u => u.UserId == Convert.ToInt32(userId)).FirstOrDefault();
            user.Notifications = value == "True" ? 1 : 0;

            await _context.SaveChangesAsync();

            return await Task.FromResult(user);
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
