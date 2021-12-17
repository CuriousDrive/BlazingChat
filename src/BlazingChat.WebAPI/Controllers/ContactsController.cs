using BlazingChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlazingChat.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BlazingChat.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ContactsController : ControllerBase
    {
        private readonly ILogger<ContactsController> logger;
        private readonly BlazingChatContext _context;

        public ContactsController(ILogger<ContactsController> logger, BlazingChatContext context)
        {
            this.logger = logger;
            this._context = context;
        }

        //loading 20,000 contacts
        [HttpGet("getallcontactsdemo")]
        public List<User> GetAllContactsDemo()
        {
            List<User> users = new();
            users.AddRange(Enumerable.Range(0, 20001).Select(x => new User { UserId = x, FirstName = $"First{x}", LastName = $"Last{x}" }));

            return users;

        }

        [HttpGet("getonlyvisiblecontactsdemo")]
        public List<User> GetOnlyVisibleContactsDemo(int startIndex, int count)
        {
            List<User> users = new();
            users.AddRange(Enumerable.Range(startIndex, count).Select(x => new User { UserId = x, FirstName = $"First{x}", LastName = $"Last{x}" }));

            return users;
        }

        //loading actual contacts
        [HttpGet("getcontacts")]
        public async Task<List<User>> GetContacts()
        {
            return await _context.Users.Select(user => new User
            {
                UserId = user.UserId,
                EmailAddress = user.EmailAddress,
                FirstName = user.FirstName,
                LastName = user.LastName
            }).ToListAsync();
        }

        [HttpGet("getvisiblecontacts")]
        public async Task<List<User>> GetVisibleContacts(int startIndex, int count)
        {
            return await _context.Users
                                .Skip(startIndex)
                                .Take(count)
                                .Select(user => new User
                                {
                                    UserId = user.UserId,
                                    EmailAddress = user.EmailAddress,
                                    FirstName = user.FirstName,
                                    LastName = user.LastName
                                }).ToListAsync();
        }

        [HttpGet("getcontactscount")]
        public async Task<int> GetContactsCount()
        {
            //throw new IndexOutOfRangeException();
            return await _context.Users.CountAsync();
        }
    }
}
