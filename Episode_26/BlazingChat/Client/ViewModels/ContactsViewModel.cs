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
        public async Task<List<Contact>> GetContacts()
        {
            List<User> users = await _httpClient.GetFromJsonAsync<List<User>>("contacts/getcontacts");
            LoadCurrentObject(users);

            return this.Contacts;
        }

        private void LoadCurrentObject(List<User> users)
        {
            this.Contacts = new List<Contact>();
            foreach (User user in users)
            {
                this.Contacts.Add(user);
            }
        }

        public async Task<List<Contact>> GetAllContacts()
        {
            List<User> users = await _httpClient.GetFromJsonAsync<List<User>>("contacts/getallcontacts");
            LoadCurrentObject(users);
            return Contacts;
        }

        public async Task<List<Contact>> GetOnlyVisibleContacts(int startIndex, int count)
        {
            List<User> users = await _httpClient.GetFromJsonAsync<List<User>>($"contacts/getonlyvisiblecontacts?startIndex={startIndex}&count={count}");

            LoadCurrentObject(users);
            return Contacts;
        }

        public async Task<int> GetContactsCount()
        {
            return await _httpClient.GetFromJsonAsync<int>($"contacts/getcontactscount");
        }

        public async Task<List<Contact>> GetVisibleContacts(int startIndex, int count)
        {
            List<User> users = await _httpClient.GetFromJsonAsync<List<User>>($"contacts/getvisiblecontacts?startIndex={startIndex}&count={count}");

            LoadCurrentObject(users);
            return Contacts;
        }
    }
}
