using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BlazingChat.Client;
using BlazingChat.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazingChat.ViewModels
{
    public class LoginViewModel : ILoginViewModel
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }

        private HttpClient _httpClient;
        AuthenticationStateProvider _authenticationStateProvider;
        public LoginViewModel()
        {

        }
        public LoginViewModel(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task LoginUser()
        {
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync<User>("user/loginuser", this);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                User user = await httpResponseMessage.Content.ReadFromJsonAsync<User>();
                ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedIn(user);
            }
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