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
            AuthenticationRequest authenticationRequest = new AuthenticationRequest();
            authenticationRequest.EmailAddress = this.EmailAddress;
            authenticationRequest.Password = this.Password;
            
            var httpMessageReponse = await _httpClient.PostAsJsonAsync<AuthenticationRequest>($"user/authenticatejwt", authenticationRequest);
            return await httpMessageReponse.Content.ReadFromJsonAsync<AuthenticationResponse>();
        }

        public static implicit operator LoginViewModel(User user)
        {
            return new LoginViewModel
            {
                EmailAddress = user.EmailAddress,
                Password = user.Password
            };
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