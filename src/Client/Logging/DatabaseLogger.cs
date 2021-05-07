using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazingChat.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace BlazingChat.Client.Logging
{
    public class DatabaseLogger : ILogger
    {
        private readonly HttpClient _httpClient;

        public AuthenticationStateProvider _authStateProvider { get; }

        public DatabaseLogger(HttpClient httpClient, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
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
            Task.Run(async () => { 

                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                long userId = 0;
                if (user.Identity.IsAuthenticated)
                    userId = Convert.ToInt64(user.FindFirst(ClaimTypes.NameIdentifier).Value);

                Log log = new();
                log.LogLevel = logLevel.ToString();
                log.UserId = userId;
                log.ExceptionMessage = exception?.Message;
                log.StackTrace = exception?.StackTrace;
                log.Source = "Client";
                log.CreatedDate = DateTime.Now.ToString();

                await _httpClient.PostAsJsonAsync<Log>("/logs", log);

            });
        }
    }
}