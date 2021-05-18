using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BlazingChat.Client;
using BlazingChat.Shared.Models;

namespace BlazingChat.ViewModels
{
    public class RegisterViewModel : IRegisterViewModel
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string ReenterPassword { get; set; }

        private HttpClient _httpClient;
        public RegisterViewModel()
        {

        }
        public RegisterViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task RegisterUser()
        {
            Console.WriteLine(this.EmailAddress);
            Console.WriteLine(this.Password);
            await _httpClient.PostAsJsonAsync<User>("user/registeruser", this);
        }

        public static implicit operator RegisterViewModel(User user)
        {
            return new RegisterViewModel
            {
                EmailAddress = user.EmailAddress,
                Password = user.Password
            };
        }

        public static implicit operator User(RegisterViewModel registerViewModel)
        {
            return new User
            {
                EmailAddress = registerViewModel.EmailAddress,
                Password = registerViewModel.Password
            };
        }
    }
}