using BlazingChat.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BlazingChat.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private static readonly string[] Users = new[]
        {
            "John", "Bill", "Steve", "Monica", "Rachel", "Laura", "Mark", "David", "Liz", "Amanda"
        };

        private readonly ILogger<UserController> logger;

        public UserController(ILogger<UserController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public User Get()
        {
           // Create a Random object  
            Random rand = new Random();  
            // Generate a random index less than the size of the array.  
            int index = rand.Next(Users.Length);  

            User user = new User();
            user.UserName = Users[index];
            
            return user;
            
        }
    }
}
