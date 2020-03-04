//using BlazingChat.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlazingChat.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Twitter;

namespace BlazingChat.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {        
        private readonly ILogger<UserController> logger;

        public UserController(ILogger<UserController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public Contacts Get()
        {           
           using(var blazingChatContext = new BlazingChatContext())
           {
               var contacts = blazingChatContext.Contacts.ToList();
                // Create a Random object  
                Random rand = new Random();  
                // Generate a random index less than the size of the array.  
                int index = rand.Next(contacts.Count);

                var contact = contacts.Where(c => c.ContactId == index).FirstOrDefault();

                return contact;
           }
            
        }
    
        [HttpGet("/twittersignin")]
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
    }
}
