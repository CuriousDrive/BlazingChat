using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BlazingChat.Client;
using BlazingChat.Shared;
using BlazingChat.Shared.Models;

namespace BlazingChat.ViewModels
{
    public class LoginViewModel : ILoginViewModel
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }

        private HttpClient _httpClient;
        public LoginViewModel()
        {

        }
        public LoginViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task LoginUser()
        {
            await _httpClient.PostAsJsonAsync<User>($"user/loginuser?isPersistent={this.RememberMe}", this);
        }

        public async Task<AuthenticationResponse> AuthenticateJWT()
        {
            //creating authentication request
            AuthenticationRequest authenticationRequest = new AuthenticationRequest();
            authenticationRequest.EmailAddress = this.EmailAddress;
            authenticationRequest.Password = this.Password;

            //authenticating the request
            var httpMessageReponse = await _httpClient.PostAsJsonAsync<AuthenticationRequest>($"user/authenticatejwt", authenticationRequest);

            //sending the token to the client to store
            return await httpMessageReponse.Content.ReadFromJsonAsync<AuthenticationResponse>();
        }
        public async Task<string> GetFacebookAppIDAndRedirectUriAsync()
        {
            return await _httpClient.GetStringAsync("user/getfacebookappidandredirecturi");
        }

        public async Task<User> GetUserByJWTAsync(string jwtToken)
        {
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
            if (returnedUser != null) return await Task.FromResult(returnedUser);
            else return null;
        }

        public async Task<TwitterRequestTokenResponse> GetTwitterOAuthTokenAsync()
        {
            return await _httpClient.GetFromJsonAsync<TwitterRequestTokenResponse>("user/gettwitteroauthtokenusingresharp");
        }

        public async Task<string> GetGoogleClientIDAndRedirectUriAsync()
        {
            return await _httpClient.GetStringAsync("user/getgoogleclientidandredirecturi");
        }

        public static implicit operator LoginViewModel(User user)
        {
            return new LoginViewModel
            {
                EmailAddress = user.EmailAddress,
                Password = user.Password
            };
        }

        public async Task<string> GetTwitterJWTAsync(TwitterRequestTokenResponse twitterRequestTokenResponse)
        {
            var httpMessageResponse = await _httpClient.PostAsJsonAsync<TwitterRequestTokenResponse>("user/getTwitterjwt", twitterRequestTokenResponse);
            return (await httpMessageResponse.Content.ReadFromJsonAsync<AuthenticationResponse>()).Token;
        }

        public async Task<string> GetFacebookJWTAsync(string accessToken)
        {
            var httpMessageResponse = await _httpClient.PostAsJsonAsync<FacebookAuthRequest>("user/getfacebookjwt", new FacebookAuthRequest() { AccessToken = accessToken });
            return (await httpMessageResponse.Content.ReadFromJsonAsync<AuthenticationResponse>()).Token;
        }

        public static implicit operator User(LoginViewModel loginViewModel)
        {
            return new User
            {
                EmailAddress = loginViewModel.EmailAddress,
                Password = loginViewModel.Password
            };
        }
    }
}