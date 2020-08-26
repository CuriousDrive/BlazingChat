using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazingChat.Shared.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazingChat.Client
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        public CustomAuthenticationStateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            User user = await _httpClient.GetFromJsonAsync<User>("getloggedinuser");
            ClaimsIdentity identity;

            if (user != null && user.EmailAddress != null)
                identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.EmailAddress) }, "serverauth");
            else
                identity = new ClaimsIdentity();

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void MarkUserAsLoggedIn(User user)
        {
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.FirstName) });
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var authenticationState = new AuthenticationState(claimsPrincipal);
            NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
        }

        public void MarkUserAsLoggedOut()
        {
            var identity = new ClaimsIdentity();
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var authenticationState = new AuthenticationState(claimsPrincipal);
            NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
        }
    }
}
