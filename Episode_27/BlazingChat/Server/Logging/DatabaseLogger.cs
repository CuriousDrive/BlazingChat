using System;
using BlazingChat.Server.Models;
using Microsoft.Extensions.Logging;

namespace Blazing.Server.Logging
{
    public class DatabaseLogger : ILogger
    {
        private readonly BlazingChatContext _context;

        public DatabaseLogger(BlazingChatContext context)
        {
            _context = context;
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
            Log log = new();
            log.LogLevel = logLevel.ToString();
            log.EventName = eventId.Name;
            log.LogMessage = state.ToString();
            log.StackTrace = exception?.StackTrace;

            _context.Logs.Add(log);
            _context.SaveChanges();
        }
    }
}