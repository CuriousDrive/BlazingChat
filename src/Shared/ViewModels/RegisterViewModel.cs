using BlazingChat.Shared.Models;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

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

        private readonly HttpClient _httpClient;

        public RegisterViewModel()
        {
        }
        public RegisterViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task RegisterUser() =>
            _httpClient.PostAsJsonAsync<User>("user/registeruser", this);

        public static implicit operator RegisterViewModel(User user) =>
            new()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                Password = user.Password
            };

        public static implicit operator User(RegisterViewModel registerViewModel) =>
            new()
            {
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                EmailAddress = registerViewModel.EmailAddress,
                Password = registerViewModel.Password
            };
    }
}
