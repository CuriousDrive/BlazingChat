using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using BlazingChat.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace BlazingChat.ViewModels
{
    public class LoginViewModel : ILoginViewModel
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }

        private HttpClient _httpClient;
        private NavigationManager _navigationManager;
        public LoginViewModel()        
        {
                
        }
        public LoginViewModel(HttpClient httpClient, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
        }

        public void LoginUser()
        {
            _navigationManager.NavigateTo("/profile");
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