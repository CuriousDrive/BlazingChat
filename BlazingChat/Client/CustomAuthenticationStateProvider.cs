using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazingChat.Shared.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazingChat.Client
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;

        public CustomAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = await _httpClient.GetFromJsonAsync<User>("user");

            var IsAuthenticated = false;

            if (user != null && user.EmailAddress != null)
            {
                IsAuthenticated = true;
                await _localStorageService.SetItemAsync("userId", user.UserId);
            }                

            var identity = IsAuthenticated
                 ? new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.EmailAddress) }, "serverauth")
                 : new ClaimsIdentity();            
            
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task MarkUserAsLoggedIn(User user)
        {
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.EmailAddress) }, "serverauth");

            var claimsPrincipal = new ClaimsPrincipal(identity);

            await _localStorageService.SetItemAsync("userId", user.UserId);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

         public async Task MarkUserAsLoggedOut()
        {
            var identity = new ClaimsIdentity();

            var claimsPrincipal = new ClaimsPrincipal(identity);

            await _localStorageService.RemoveItemAsync("userId");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }
}
