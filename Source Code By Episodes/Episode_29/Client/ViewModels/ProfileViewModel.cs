using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazingChat.Shared.Models;

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
        private HttpClient _httpClient;

        public ProfileViewModel()
        {

        }

        public ProfileViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task UpdateProfile()
        {
            User user = this;
            await _httpClient.PutAsJsonAsync("profile/updateprofile/" + this.UserId, user);
            this.Message = "Profile updated successfully";
        }

        public async Task GetProfile()
        {
            User user = await _httpClient.GetFromJsonAsync<User>("profile/getprofile/" + this.UserId);
            LoadCurrentObject(user);
            this.Message = "Profile loaded successfully";    
        }
        private void LoadCurrentObject(ProfileViewModel profileViewModel)
        {
            this.FirstName = profileViewModel.FirstName;
            this.LastName = profileViewModel.LastName;
            this.EmailAddress = profileViewModel.EmailAddress;
            this.AboutMe = profileViewModel.AboutMe;
            this.ProfilePicDataUrl = profileViewModel.ProfilePicDataUrl;
            //add more fields
        }

        public static implicit operator ProfileViewModel(User user)
        {
            return new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                UserId = user.UserId,
                AboutMe = user.AboutMe,
                ProfilePicDataUrl = user.ProfilePicDataUrl
            };
        }

        public static implicit operator User(ProfileViewModel profileViewModel)
        {
            return new User
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
}