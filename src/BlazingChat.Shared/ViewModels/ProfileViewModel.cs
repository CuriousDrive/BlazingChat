using BlazingChat.Shared.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

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

        public ProfileViewModel()
        {
        }

        public ProfileViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task UpdateProfile() =>
            _httpClient.PutAsJsonAsync($"profile/updateprofile/{UserId}", this);

        public async Task GetProfile()
        {
            ProfileViewModel user = await _httpClient.GetFromJsonAsync<User>($"profile/getprofile/{UserId}");
            LoadCurrentObject(user);
            Message = "Profile loaded successfully";
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
