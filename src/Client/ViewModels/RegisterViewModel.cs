using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BlazingChat.Client;
using BlazingChat.Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace BlazingChat.ViewModels
{
    public class RegisterViewModel : IRegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }
        public string ReenterPassword { get; set; }
        public int PasswordLength => Password.Length;

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
            await _httpClient.PostAsJsonAsync<User>("user/registeruser", this);
        }

        public static implicit operator RegisterViewModel(User user)
        {
            return new RegisterViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                Password = user.Password
            };
        }

        public static implicit operator User(RegisterViewModel registerViewModel)
        {
            return new User
            {
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                EmailAddress = registerViewModel.EmailAddress,
                Password = registerViewModel.Password
            };
        }
    }
}