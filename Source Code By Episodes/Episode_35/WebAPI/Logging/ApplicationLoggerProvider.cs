using BlazingChat.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BlazingChat.WebAPI.Logging
{
    public class ApplicationLoggerProvider : ILoggerProvider
    {
        private readonly BlazingChatContext _context;

        public IHttpContextAccessor _httpContextAccessor { get; }

        public ApplicationLoggerProvider(BlazingChatContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new DatabaseLogger(_context, _httpContextAccessor);
        }

        public void Dispose()
        {

        }
    }
}