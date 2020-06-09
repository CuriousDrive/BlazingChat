using BlazingChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlazingChat.Server.Models;

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

        public UserController(ILogger<UserController> logger, BlazingChatContext _blazingChatContext)
        {
            this.logger = logger;
            this._context = _blazingChatContext;
        }

        [HttpGet]
        public List<User> Get()
        {
            return _context.Users.ToList();
        }
    }
}
