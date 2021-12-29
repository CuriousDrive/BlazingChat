using BlazingChat.Shared.Extensions;
using BlazingChat.Shared.Models;
using BlazingChat.Shared.Services;
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
        private readonly IAccessTokenService _accessTokenService;


        public SettingsViewModel()
        {
        }

        public SettingsViewModel(HttpClient httpClient, 
            IAccessTokenService accessTokenService)
        {
            _httpClient = httpClient;
            _accessTokenService = accessTokenService;
        }

        public async Task GetProfile()
        {
            var jwtToken = await _accessTokenService.GetAccessTokenAsync("jwt_token");
            var user = await _httpClient.GetAsync<User>($"profile/getprofile/{UserId}", jwtToken);
            LoadCurrentObject(user);
        }

        public async Task UpdateTheme()
        {
            var jwtToken = await _accessTokenService.GetAccessTokenAsync("jwt_token");
            User user = this;
            await _httpClient.PutAsync<User>($"settings/updatetheme/{UserId}", user, jwtToken);
        }

        public async Task UpdateNotifications()
        {
            var jwtToken = await _accessTokenService.GetAccessTokenAsync("jwt_token");
            User user = this;
            await _httpClient.PutAsync<User>($"settings/updatenotifications/{UserId}", user, jwtToken);
        }

        private void LoadCurrentObject(SettingsViewModel settingsViewModel)
        {
            DarkTheme = settingsViewModel.DarkTheme;
            Notifications = settingsViewModel.Notifications;
        }

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
