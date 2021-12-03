using System;
using BlazingChat.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BlazingChat.WebAPI.Logging
{
    public class DatabaseLogger : ILogger
    {
        private readonly IDbContextFactory<LoggingBlazingChatContext> _contextFactory;

        public DatabaseLogger(IDbContextFactory<LoggingBlazingChatContext> contextFactory)
        {
            _contextFactory = contextFactory;
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
            //No need to log UserId as the userId is already coming from the client error log
            long userId = 0;

            Log log = new();
            log.LogLevel = logLevel.ToString();
            log.UserId = Convert.ToInt64(userId);
            log.ExceptionMessage = exception?.Message;
            log.StackTrace = exception?.StackTrace;
            log.Source = "Server";
            log.CreatedDate = DateTime.Now.ToString();

            using var context = _contextFactory.CreateDbContext();
            context.Logs.Add(log);
            context.SaveChanges();
        }
    }
}
