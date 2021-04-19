using System.Collections.Concurrent;
using BlazingChat.Server.Models;
using Microsoft.Extensions.Logging;

namespace Blazing.Server.Logging
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