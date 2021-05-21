using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BlazingChat.Shared.Models;
using Blazored.Toast.Services;

namespace BlazingChat.ViewModels
{
    public class SettingsViewModel : ISettingsViewModel
    {
        //properties
        public long UserId { get; set; }
        public bool Notifications { get; set; }
        public bool DarkTheme { get; set; }
        private HttpClient _httpClient;
        public IToastService _toastService { get; }

        //methods
        public SettingsViewModel()
        {
        }
        public SettingsViewModel(HttpClient httpClient, IToastService toastService)
        {
            _httpClient = httpClient;
            _toastService = toastService;
        }
        public async Task GetProfile()
        {
            User user = await _httpClient.GetFromJsonAsync<User>($"profile/getprofile/{this.UserId}");
            LoadCurrentObject(user);
        }
        public async Task Save()
        {
            User user = this;
            await _httpClient.PutAsJsonAsync($"settings/updatetheme/{this.UserId}", user);
            await _httpClient.PutAsJsonAsync($"settings/updatenotifications/{this.UserId}", user);
            _toastService.ShowSuccess("Settings have been saved successfully");
        }
        private void LoadCurrentObject(SettingsViewModel settingsViewModel)
        {
            this.DarkTheme = settingsViewModel.DarkTheme;
            this.Notifications = settingsViewModel.Notifications;
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
