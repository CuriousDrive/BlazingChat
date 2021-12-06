using System.Threading.Tasks;

namespace BlazingChat.ViewModels
{
    public interface ISettingsViewModel
    {
        public long UserId { get; set; }
        public bool Notifications { get; set; }
        public bool DarkTheme { get; set; }

        public Task UpdateTheme();
        public Task UpdateNotifications();
        public Task GetProfile();
    }
}
