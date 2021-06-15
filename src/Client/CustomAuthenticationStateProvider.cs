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
            User currentUser = await _httpClient.GetFromJsonAsync<User>("user/getcurrentuser");
            //User currentUser = await GetUserByJWTAsync();

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

        /// <summary>
        /// This method will get the user from server side based on the JWT stored in 
        /// the local storage
        /// </summary>
        /// <returns></returns>
        public async Task<User> GetUserByJWTAsync()
        {
            var jwtToken = await _localStorageService.GetItemAsStringAsync("jwt_token");

            string serializedJWT = JsonSerializer.Serialize(jwtToken);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "users/getcurrentuserjwt");
            requestMessage.Content = new StringContent(serializedJWT);

            requestMessage.Content.Headers.ContentType
                = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(requestMessage);

            var responseStatusCode = response.StatusCode;
            var responseBody = await response.Content.ReadAsStringAsync();

            var returnedUser = JsonSerializer.Deserialize<User>(responseBody);

            Console.WriteLine(returnedUser.UserId);

            return await Task.FromResult(returnedUser);
        }
    }
}