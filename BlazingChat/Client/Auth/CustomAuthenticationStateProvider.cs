using System.Net.Http;
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

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var contact = await _httpClient.GetJsonAsync<Contacts>("user");
            var IsAuthenticated = false;

            if (contact.FirstName != null)
                IsAuthenticated = true;

            var identity = IsAuthenticated
                 ? new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, contact.FirstName) }, "serverauth")
                 : new ClaimsIdentity();

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
    }
}
