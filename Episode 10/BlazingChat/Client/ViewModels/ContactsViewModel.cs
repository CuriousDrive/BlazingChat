using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BlazingChat.Shared.Models;

namespace BlazingChat.ViewModels
{
    public class ContactsViewModel : IContactsViewModel
    {
        //properties
        public List<Contact> Contacts { get; set; }
        private HttpClient _httpClient;

        //methods
        public ContactsViewModel()
        {            
        }
        public ContactsViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task GetContacts()
        {
            User user = await _httpClient.GetFromJsonAsync<User>("user/getprofile/10");
            LoadCurrentObject(user);
        }
        
        private void LoadCurrentObject(ContactsViewModel contactsViewModel)
        {
            //this.DarkTheme = settingsViewModel.DarkTheme;
            //this.Notifications = settingsViewModel.Notifications;
        }

        //operators
        public static implicit operator ContactsViewModel(User user)
        {
            return new ContactsViewModel
            {
                //Notifications = (user.Notifications == null || (long)user.Notifications == 0) ? false : true,
                //DarkTheme = (user.DarkTheme == null || (long)user.DarkTheme == 0) ? false : true
            };
        }
        public static implicit operator User(ContactsViewModel contactsViewModel)
        {
            return new User
            {
                //Notifications = settingsViewModel.Notifications ? 1 : 0,
                //DarkTheme = settingsViewModel.DarkTheme ? 1 : 0
            };
        }
    }
}
