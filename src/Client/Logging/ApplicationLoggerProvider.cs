using System.Net.Http;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace BlazingChat.Client.Logging
{
    public class ApplicationLoggerProvider : ILoggerProvider
    {
        private readonly HttpClient _httpClient;

        public AuthenticationStateProvider _authStateProvider { get; }

        public ApplicationLoggerProvider(HttpClient httpClient, AuthenticationStateProvider authStateProvider)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;

        }
        public ILogger CreateLogger(string categoryName)
        {
            return new DatabaseLogger(_httpClient, _authStateProvider);
        }

        public void Dispose()
        {
            
        }
    }
}