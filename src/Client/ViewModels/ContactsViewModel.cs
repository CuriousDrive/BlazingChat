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
        public List<Contact> Contacts { get; set; } = new List<Contact>();
        public int ContactsCount { get; set; } = 1;
        private HttpClient _httpClient;

        //methods
        public ContactsViewModel() { }
        public ContactsViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private void LoadCurrentObject(List<User> users)
        {
            this.Contacts = new List<Contact>();
            foreach (User user in users)
            {
                this.Contacts.Add(user);
            }
        }

        //loading 20,000 contacts
        public async Task LoadAllContactsDemo()
        {
            List<User> users = await _httpClient.GetFromJsonAsync<List<User>>("contacts/getallcontactsdemo");
            LoadCurrentObject(users);
        }

        public async Task LoadOnlyVisibleContactsDemo(int startIndex, int count)
        {
            List<User> users = await _httpClient.GetFromJsonAsync<List<User>>($"contacts/getonlyvisiblecontactsdemo?startIndex={startIndex}&count={count}");
            LoadCurrentObject(users);
        }
        public void LoadContactsCountDemo()
        {
            this.ContactsCount = 20000;
        }

        //loading actual contacts
        public async Task LoadAllContactsDB()
        {
            List<User> users = await _httpClient.GetFromJsonAsync<List<User>>("contacts/getcontacts");
            LoadCurrentObject(users);
        }

        public async Task LoadOnlyVisibleContactsDB(int startIndex, int count)
        {
            List<User> users = await _httpClient.GetFromJsonAsync<List<User>>($"contacts/getvisiblecontacts?startIndex={startIndex}&count={count}");
            LoadCurrentObject(users);
        }
        public async Task LoadContactsCountDB()
        {
            this.ContactsCount = await _httpClient.GetFromJsonAsync<int>($"contacts/getcontactscount");
        }
    }
}
