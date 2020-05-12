using System;
using System.Collections.Generic;
using System.Text;
using BlazingChat.Shared.Models;

namespace BlazingChat.Shared.ViewModels
{
    public class SettingsViewModel
    {
        public bool Notifications { get; set; }
        public bool DarkTheme { get; set; }

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
                DarkTheme =  settingsViewModel.DarkTheme ? 1 : 0
            };
        }
    }
}
