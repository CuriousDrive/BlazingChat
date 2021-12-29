using BlazingChat.Shared;
using BlazingChat.Shared.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazingChat.ViewModels
{
    public class LoginViewModel : ILoginViewModel
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }

        private readonly HttpClient _httpClient;

        public LoginViewModel()
        {
        }
        public LoginViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task LoginUser() =>
            _httpClient.PostAsJsonAsync<User>($"user/loginuser?isPersistent={RememberMe}", this);

        public async Task<AuthenticationResponse> AuthenticateJWT()
        {
            //creating authentication request
            var authenticationRequest = new AuthenticationRequest
            {
                EmailAddress = EmailAddress,
                Password = Password,
            };

            //authenticating the request
            var httpMessageResponse = await _httpClient.PostAsJsonAsync("user/authenticatejwt", authenticationRequest);

            //sending the token to the client to store
            return await httpMessageResponse.Content.ReadFromJsonAsync<AuthenticationResponse>();
        }

        public Task<string> GetFacebookAppIDAndRedirectUriAsync() =>
            _httpClient.GetStringAsync("user/getfacebookappidandredirecturi");

        public async Task<User> GetUserByJWTAsync(string jwtToken)
        {
            try
            {
                //preparing the http request
                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, "user/getuserbyjwt")
                {
                    Content = new StringContent(jwtToken)
                    {
                        Headers =
                        {
                            ContentType = new MediaTypeHeaderValue("application/json")
                        }
                    }
                };

                //making the http request
                var response = await _httpClient.SendAsync(requestMessage);

                //returning the user if found
                var returnedUser = await response.Content.ReadFromJsonAsync<User>();
                if (returnedUser != null) return await Task.FromResult(returnedUser);
                else return null;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.GetType());
                return null;
            }            
        }

        public Task<TwitterRequestTokenResponse> GetTwitterOAuthTokenAsync() =>
            _httpClient.GetFromJsonAsync<TwitterRequestTokenResponse>("user/gettwitteroauthtokenusingresharp");

        public Task<string> GetGoogleClientIDAndRedirectUriAsync() =>
            _httpClient.GetStringAsync("user/getgoogleclientidandredirecturi");

        public static implicit operator LoginViewModel(User user) =>
            new()
            {
                EmailAddress = user.EmailAddress,
                Password = user.Password
            };

        public async Task<string> GetTwitterJWTAsync(TwitterRequestTokenResponse twitterRequestTokenResponse)
        {
            using var httpMessageResponse = await _httpClient.PostAsJsonAsync("user/getTwitterjwt", twitterRequestTokenResponse);

            return (await httpMessageResponse.Content.ReadFromJsonAsync<AuthenticationResponse>()).Token;
        }

        public async Task<string> GetFacebookJWTAsync(string accessToken)
        {
            using var httpMessageResponse = await _httpClient.PostAsJsonAsync("user/getfacebookjwt", new FacebookAuthRequest { AccessToken = accessToken });

            return (await httpMessageResponse.Content.ReadFromJsonAsync<AuthenticationResponse>()).Token;
        }

        public static implicit operator User(LoginViewModel loginViewModel) =>
            new()
            {
                EmailAddress = loginViewModel.EmailAddress,
                Password = loginViewModel.Password
            };
    }
}
