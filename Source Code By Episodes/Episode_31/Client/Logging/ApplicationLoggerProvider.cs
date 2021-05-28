using System.Net.Http;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

namespace BlazingChat.Client.Logging
{
    public class ApplicationLoggerProvider : ILoggerProvider
    {
        private readonly HttpClient _httpClient;

        public AuthenticationStateProvider _authenticationStateProvider { get; }

        public ApplicationLoggerProvider(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new DatabaseLogger(_httpClient, _authenticationStateProvider);
        }

        public void Dispose()
        {
            
        }
    }
}