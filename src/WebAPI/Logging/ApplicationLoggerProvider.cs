using BlazingChat.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BlazingChat.WebAPI.Logging
{
    public class ApplicationLoggerProvider : ILoggerProvider
    {
        private readonly BlazingChatContext _context;

        public ApplicationLoggerProvider(BlazingChatContext context)
        {
            _context = context;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new DatabaseLogger(_context);
        }

        public void Dispose()
        {

        }
    }
}