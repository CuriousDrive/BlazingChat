using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BlazingChat.Shared.Models;

namespace BlazingChat.ViewModels
{
    public class SettingsViewModel : ISettingsViewModel
    {
        public bool Notifications { get; set; }
        public bool DarkTheme { get; set; }

        private HttpClient _httpClient;

        public SettingsViewModel()
        {            
        }
        public SettingsViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task UpdateTheme(int userId)
        {
            User user = await _httpClient.GetFromJsonAsync<User>($"user/updatetheme?userId={userId}&value={this.DarkTheme.ToString()}");
        }

        public async Task UpdateNotifications(int userId)
        {
            User user = await _httpClient.GetFromJsonAsync<User>($"user/updatenotifications?userId={userId}&value={this.Notifications.ToString()}");
        }

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
