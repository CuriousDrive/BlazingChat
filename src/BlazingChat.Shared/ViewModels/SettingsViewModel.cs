using BlazingChat.Shared.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazingChat.ViewModels
{
    public class SettingsViewModel : ISettingsViewModel
    {
        public long UserId { get; set; }
        public bool Notifications { get; set; }
        public bool DarkTheme { get; set; }

        private readonly HttpClient _httpClient;

        public SettingsViewModel()
        {
        }

        public SettingsViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task GetProfile() =>
            LoadCurrentObject(await _httpClient.GetFromJsonAsync<User>($"profile/getprofile/{UserId}"));

        public Task UpdateTheme() =>
            _httpClient.PutAsJsonAsync<User>($"settings/updatetheme/{UserId}", this);

        public Task UpdateNotifications() =>
            _httpClient.PutAsJsonAsync<User>($"settings/updatenotifications/{UserId}", this);

        private void LoadCurrentObject(SettingsViewModel settingsViewModel) =>
            DarkTheme = settingsViewModel.DarkTheme;

        public static implicit operator SettingsViewModel(User user) =>
            new()
            {
                Notifications = user.Notifications != null && (long)user.Notifications != 0,
                DarkTheme = user.DarkTheme != null && (long)user.DarkTheme != 0
            };

        public static implicit operator User(SettingsViewModel settingsViewModel) =>
            new()
            {
                Notifications = settingsViewModel.Notifications ? 1 : 0,
                DarkTheme = settingsViewModel.DarkTheme ? 1 : 0
            };
    }
}
