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
            List<User> users = await _httpClient.GetFromJsonAsync<List<User>>("user/getcontacts");            
            LoadCurrentObject(users);
        }
        
        private void LoadCurrentObject(List<User> users)
        {
            this.Contacts = new List<Contact>();
            foreach(User user in users)
            {
                this.Contacts.Add(user);
            }
        }

       
    }
}
