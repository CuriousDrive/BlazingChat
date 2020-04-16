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

namespace BlazingChat.Server.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    public class UserController : ControllerBase
    {        
        private readonly ILogger<UserController> _logger;
        private readonly BlazingChatContext _context;

        public UserController(ILogger<UserController> logger,BlazingChatContext context)
        {
            this._logger = logger;
            this._context = context;
        }

        [HttpGet("user")]
        public Contacts Get()
        {           
            var contact = new Contacts();

            if(User.Identity.IsAuthenticated)
            {
                contact.FirstName = User.Identity.Name;   
            }

            return contact;
        }

        [HttpPost("user/createuser")]
        public async Task CreateUser(User user)
        {   
            user.UserId = _context.User.Max(u => u.UserId) + 1;
            user.Source = "APPL";
            _context.User.Add(user);

            await _context.SaveChangesAsync();
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
