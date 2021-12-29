using System;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazingChat.Shared.Models;
using BlazingChat.Shared.Services;
using BlazingChat.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazingChat.Client
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILoginViewModel _loginViewModel;
        private readonly IAccessTokenService _accessTokenService;

        public CustomAuthenticationStateProvider(ILoginViewModel loginViewModel, 
            IAccessTokenService accessTokenService)
        {
            _loginViewModel = loginViewModel;
            _accessTokenService = accessTokenService;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            User currentUser = await GetUserByJWTAsync();

            if (currentUser != null && currentUser.EmailAddress != null)
            {
                //create claimsPrincipal
                var claimsPrincipal = GetClaimsPrinciple(currentUser);
                return new AuthenticationState(claimsPrincipal);
            }
            else
            {
                await _accessTokenService.RemoveAccessTokenAsync("jwt_token");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task MarkUserAsAuthenticated()
        {
            var user = await GetUserByJWTAsync();
            var claimsPrincipal = GetClaimsPrinciple(user);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _accessTokenService.RemoveAccessTokenAsync("jwt_token");

            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public async Task<User> GetUserByJWTAsync()
        {
            //pulling the token from localStorage
            var jwtToken = await _accessTokenService.GetAccessTokenAsync("jwt_token");
            if (jwtToken == null) return null;

            jwtToken = $@"""{jwtToken}""";
            return await _loginViewModel.GetUserByJWTAsync(jwtToken);
        }

        private ClaimsPrincipal GetClaimsPrinciple(User currentUser)
        {
            //create a claims
            var claimEmailAddress = new Claim(ClaimTypes.Name, currentUser.EmailAddress);
            var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, Convert.ToString(currentUser.UserId));
            var claimRole = new Claim(ClaimTypes.Role, currentUser.Role == null ? "" : currentUser.Role);

            //create claimsIdentity
            var claimsIdentity = new ClaimsIdentity(new[] { claimEmailAddress, claimNameIdentifier, claimRole }, "serverAuth");
            //create claimsPrincipal
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return claimsPrincipal;
        }


    }
}