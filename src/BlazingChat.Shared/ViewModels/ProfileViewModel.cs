using BlazingChat.Shared.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazingChat.Shared.Extensions;
using BlazingChat.Shared.Services;

namespace BlazingChat.ViewModels
{
    public class ProfileViewModel : IProfileViewModel
    {
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Message { get; set; }
        public string AboutMe { get; set; }
        public string ProfilePicDataUrl { get; set; }

        private readonly HttpClient _httpClient;
        private readonly IAccessTokenService _accessTokenService;

        public ProfileViewModel()
        {
        }

        public ProfileViewModel(HttpClient httpClient, 
            IAccessTokenService accessTokenService)
        {
            _httpClient = httpClient;
            _accessTokenService = accessTokenService;
        }

        public async Task UpdateProfile()
        {
            var jwtToken = await _accessTokenService.GetAccessTokenAsync("jwt_token");
            await _httpClient.PutAsync<User>($"profile/updateprofile/{UserId}", this, jwtToken);
        }

        public async Task GetProfile()
        {
            var jwtToken = await _accessTokenService.GetAccessTokenAsync("jwt_token");
            ProfileViewModel user = await _httpClient.GetAsync<User>($"profile/getprofile/{UserId}",jwtToken);
            LoadCurrentObject(user);
        }

        private void LoadCurrentObject(IProfileViewModel profileViewModel)
        {
            FirstName = profileViewModel.FirstName;
            LastName = profileViewModel.LastName;
            EmailAddress = profileViewModel.EmailAddress;
            AboutMe = profileViewModel.AboutMe;
            ProfilePicDataUrl = profileViewModel.ProfilePicDataUrl;
            //add more fields
        }

        public static implicit operator ProfileViewModel(User user) =>
            new()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                UserId = user.UserId,
                AboutMe = user.AboutMe,
                ProfilePicDataUrl = user.ProfilePicDataUrl
            };

        public static implicit operator User(ProfileViewModel profileViewModel) =>
            new()
            {
                FirstName = profileViewModel.FirstName,
                LastName = profileViewModel.LastName,
                EmailAddress = profileViewModel.EmailAddress,
                UserId = profileViewModel.UserId,
                AboutMe = profileViewModel.AboutMe,
                ProfilePicDataUrl = profileViewModel.ProfilePicDataUrl
            };
    }
}
