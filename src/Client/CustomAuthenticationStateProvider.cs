using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using BlazingChat.Shared.Models;
using Blazored.LocalStorage;
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

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            User currentUser = await GetUserByJWTAsync(); //_httpClient.GetFromJsonAsync<User>("user/getcurrentuser");

            if (currentUser != null && currentUser.EmailAddress != null)
            {
                //create a claims
                var claimEmailAddress = new Claim(ClaimTypes.Name, currentUser.EmailAddress);
                var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, Convert.ToString(currentUser.UserId));
                //create claimsIdentity
                var claimsIdentity = new ClaimsIdentity(new[] { claimEmailAddress, claimNameIdentifier }, "serverAuth");
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
            if(jwtToken == null) return null;
        
            //preparing the http request
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "user/getuserbyjwt");
            requestMessage.Content = new StringContent(jwtToken);
        
            requestMessage.Content.Headers.ContentType
                = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        
            //making the http request
            var response = await _httpClient.SendAsync(requestMessage);
        
            var responseStatusCode = response.StatusCode;
            var returnedUser = await response.Content.ReadFromJsonAsync<User>();
        
            //returning the user if found
            if(returnedUser != null) return await Task.FromResult(returnedUser);
            else return null;
        }
    }
}