using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazingChat.Shared.Models;
using BlazingChat.ViewModels;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazingChat.Client
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILoginViewModel _loginViewModel;
        private readonly ILocalStorageService _localStorageService;

        public CustomAuthenticationStateProvider(ILoginViewModel loginViewModel, ILocalStorageService localStorageService)
        {
            _loginViewModel = loginViewModel;
            _localStorageService = localStorageService;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            User currentUser = await GetUserByJWTAsync();

            if (currentUser != null && currentUser.EmailAddress != null)
            {
                //create a claims
                var claimEmailAddress = new Claim(ClaimTypes.Name, currentUser.EmailAddress);
                var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, Convert.ToString(currentUser.UserId));
                var claimRole = new Claim(ClaimTypes.Role, currentUser.Role == null ? "" : currentUser.Role);

                //create claimsIdentity
                var claimsIdentity = new ClaimsIdentity(new[] { claimEmailAddress, claimNameIdentifier, claimRole }, "serverAuth");
                //create claimsPrincipal
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                return new AuthenticationState(claimsPrincipal);
            }
            else
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public async Task<User> GetUserByJWTAsync()
        {
            //pulling the token from localStorage
            var jwtToken = await _localStorageService.GetItemAsStringAsync("jwt_token");
            if (jwtToken == null) return null;

            return await _loginViewModel.GetUserByJWTAsync(jwtToken);
        }
    }
}