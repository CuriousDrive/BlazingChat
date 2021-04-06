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
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace BlazingChat.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly BlazingChatContext _context;

        public UserController(ILogger<UserController> logger, BlazingChatContext context)
        {
            this.logger = logger;
            this._context = context;
        }

        [HttpGet("getcontacts")]
        public List<User> GetContacts()
        {
            return _context.Users.ToList();
        }

        //Authentication Methods
        [HttpPost("loginuser")]
        public async Task<ActionResult<User>> LoginUser(User user)
        {
            user.Password = Utility.Encrypt(user.Password);
            User loggedInUser = await _context.Users.Where(u => u.EmailAddress == user.EmailAddress && u.Password == user.Password).FirstOrDefaultAsync();

            if (loggedInUser != null)
            {
                //create a claim
                var claimEmail = new Claim(ClaimTypes.Email, loggedInUser.EmailAddress);
                var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, loggedInUser.UserId.ToString());
                //create claimsIdentity
                var claimsIdentity = new ClaimsIdentity(new[] { claimEmail, claimNameIdentifier }, "serverAuth");
                //create claimsPrincipal
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                //Sign In User
                await HttpContext.SignInAsync(claimsPrincipal, GetAuthenticationProperties());
            }
            return await Task.FromResult(loggedInUser);
        }

        [HttpGet("getcurrentuser")]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            User currentUser = new User();

            if (User.Identity.IsAuthenticated)
            {
                currentUser.EmailAddress = User.FindFirstValue(ClaimTypes.Email);
                currentUser = await _context.Users.Where(u => u.EmailAddress == currentUser.EmailAddress).FirstOrDefaultAsync();

                if (currentUser == null)
                {
                    currentUser = new User();
                    currentUser.UserId = _context.Users.Max(user => user.UserId) + 1;
                    currentUser.EmailAddress = User.FindFirstValue(ClaimTypes.Email);
                    currentUser.Password = Utility.Encrypt(currentUser.EmailAddress);
                    currentUser.Source = "EXTL";

                    _context.Users.Add(currentUser);
                    await _context.SaveChangesAsync();
                }
            }
            return await Task.FromResult(currentUser);
        }

        [HttpGet("logoutuser")]
        public async Task<ActionResult<String>> LogOutUser()
        {
            await HttpContext.SignOutAsync();
            return "Success";
        }

        [HttpGet("TwitterSignIn")]
        public async Task TwitterSignIn()
        {
            await HttpContext.ChallengeAsync(TwitterDefaults.AuthenticationScheme,
                GetAuthenticationProperties());
        }

        [HttpGet("FacebookSignIn")]
        public async Task FacebookSignIn()
        {
            await HttpContext.ChallengeAsync(FacebookDefaults.AuthenticationScheme,
                GetAuthenticationProperties());
        }

        [HttpGet("GoogleSignIn")]
        public async Task GoogleSignIn()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                GetAuthenticationProperties());
        }

        public AuthenticationProperties GetAuthenticationProperties()
        {
            return new AuthenticationProperties()
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.Now.AddMinutes(10),
                    RedirectUri = "/profile"
                };
        }
        [HttpGet("notauthorized")]
        public IActionResult NotAuthorized()
        {
            return Unauthorized();
        }

    }
}
