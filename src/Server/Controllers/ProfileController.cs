using BlazingChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BlazingChat.Server.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace BlazingChat.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly BlazingChatContext _context;

        public ProfileController(ILogger<UserController> logger, BlazingChatContext context)
        {
            this.logger = logger;
            this._context = context;
        }

        [HttpPut("updateprofile/{userId}")]
        public async Task<User> UpdateProfile(int userId, [FromBody] User user)
        {
            User userToUpdate = await _context.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();

            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.EmailAddress = user.EmailAddress;
            userToUpdate.AboutMe = user.AboutMe;
            userToUpdate.ProfilePicDataUrl = user.ProfilePicDataUrl;

            await _context.SaveChangesAsync();

            return await Task.FromResult(user);
        }

        [HttpGet("getprofile/{userId}")]
        public async Task<User> GetProfile(int userId)
        {
            return await _context.Users.Where(u => u.UserId == userId).FirstOrDefaultAsync();
        }

        [HttpGet("DownloadServerFile")]
        public async Task<string> DownloadServerFile()
        {
            var filePath = @"C:\Data\CuriousDrive\GitHub Repos\BlazingChat\Documents\Word\ServerFile.docx";

            using (var fileInput = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                MemoryStream memoryStream = new MemoryStream();
                await fileInput.CopyToAsync(memoryStream);

                var buffer = memoryStream.ToArray();
                return Convert.ToBase64String(buffer);
            }
        }
    }
}