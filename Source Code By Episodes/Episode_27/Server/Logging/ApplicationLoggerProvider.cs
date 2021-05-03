using BlazingChat.Server.Models;
using Microsoft.Extensions.Logging;

namespace BlazingChat.Server.Logging
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