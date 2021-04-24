using System;
using System.Net.Http;
using System.Net.Http.Json;
using BlazingChat.Shared.Models;
using Microsoft.Extensions.Logging;

namespace BlazingChat.Client.Logging
{
    public class DatabaseLogger : ILogger
    {
        private readonly HttpClient _httpClient;

        public DatabaseLogger(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
            log.ExceptionMessage = exception?.Message;
            log.StackTrace = exception?.StackTrace;
            log.Source = "Client";
            log.CreatedDate = DateTime.Now.ToString();

            _httpClient.PostAsJsonAsync<Log>("/logs", log);
        }
    }
}