using System;
using System.Security.Claims;
using BlazingChat.Server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BlazingChat.Server.Logging
{
    public class DatabaseLogger : ILogger
    {
        private readonly BlazingChatContext _context;

        public IHttpContextAccessor _httpContextAccessor { get; }

        public DatabaseLogger(BlazingChatContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var userId = _httpContextAccessor?.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            Log log = new();
            log.LogLevel = logLevel.ToString();
            log.UserId = Convert.ToInt64(userId);
            log.ExceptionMessage = exception?.Message;
            log.StackTrace = exception?.StackTrace;
            log.Source = "Server";
            log.CreatedDate = DateTime.Now.ToString();

            _context.Logs.Add(log);
            _context.SaveChanges();
        }
    }
}