using BlazingChat.Shared.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazingChat.ViewModels
{
    public class SettingsViewModel : ISettingsViewModel
    {
        //properties
        public long UserId { get; set; }
        public bool Notifications { get; set; }
        public bool DarkTheme { get; set; }
        private HttpClient _httpClient;

        //methods
        public SettingsViewModel()
        {
        }
        public SettingsViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task GetProfile()
        {
            User user = await _httpClient.GetFromJsonAsync<User>($"profile/getprofile/{this.UserId}");
            LoadCurrentObject(user);
        }

        public async Task UpdateTheme()
        {
            User user = this;
            await _httpClient.PutAsJsonAsync($"settings/updatetheme/{this.UserId}", user);
        }

        public async Task UpdateNotifications()
        {
            User user = this;
            await _httpClient.PutAsJsonAsync($"settings/updatenotifications/{this.UserId}", user);
        }
        private void LoadCurrentObject(SettingsViewModel settingsViewModel)
        {
            this.DarkTheme = settingsViewModel.DarkTheme;
        }

        //operators
        public static implicit operator SettingsViewModel(User user)
        {
            return new SettingsViewModel
            {
                Notifications = (user.Notifications == null || (long)user.Notifications == 0) ? false : true,
                DarkTheme = (user.DarkTheme == null || (long)user.DarkTheme == 0) ? false : true
            };
        }
        public static implicit operator User(SettingsViewModel settingsViewModel)
        {
            return new User
            {
                Notifications = settingsViewModel.Notifications ? 1 : 0,
                DarkTheme = settingsViewModel.DarkTheme ? 1 : 0
            };
        }
    }
}
