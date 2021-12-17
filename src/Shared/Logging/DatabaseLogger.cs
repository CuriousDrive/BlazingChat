using System;
using System.Text.Json;
using System.Threading.Tasks;
using BlazingChat.Shared.Logging;
using BlazingChat.Shared.Models;
using Microsoft.Extensions.Logging;
using static Microsoft.Extensions.Logging.LogLevel;

namespace BlazingChat.Shared.Logging
{
    public class DatabaseLogger : ILogger
    {
        private readonly LogWriter _logWriter;

        public DatabaseLogger(LogWriter logWriter)
        {
            _logWriter = logWriter;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel) =>
            logLevel is Error or Critical;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            Task.Run(async () =>
                await _logWriter.Write(JsonSerializer.Deserialize<LogMessage>(formatter(state, exception))));
        }
    }
}
