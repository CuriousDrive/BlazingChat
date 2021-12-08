using BlazingChat.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazingChat.ViewModels
{
    public class ContactsViewModel : IContactsViewModel
    {
        public IEnumerable<Contact> Contacts { get; private set; } = new List<Contact>();
        public int ContactsCount { get; private set; } = 1;

        private readonly HttpClient _httpClient;

        public ContactsViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private void LoadCurrentObject(IEnumerable<User> users) =>
            Contacts = users.Select(u => (Contact)u);

        //loading 20,000 contacts
        public async Task LoadAllContactsDemo() =>
            LoadCurrentObject(await _httpClient.GetFromJsonAsync<List<User>>("contacts/getallcontactsdemo"));

        public async Task LoadOnlyVisibleContactsDemo(int startIndex, int count) =>
            LoadCurrentObject(await _httpClient.GetFromJsonAsync<List<User>>($"contacts/getonlyvisiblecontactsdemo?startIndex={startIndex}&count={count}"));

        public void LoadContactsCountDemo() =>
            ContactsCount = 20000;

        //loading actual contacts
        public async Task LoadAllContactsDB() =>
            LoadCurrentObject(await _httpClient.GetFromJsonAsync<List<User>>("contacts/getcontacts"));

        public async Task LoadOnlyVisibleContactsDB(int startIndex, int count) =>
            LoadCurrentObject(await _httpClient.GetFromJsonAsync<List<User>>($"contacts/getvisiblecontacts?startIndex={startIndex}&count={count}"));

        public async Task LoadContactsCountDB() =>
            ContactsCount = await _httpClient.GetFromJsonAsync<int>("contacts/getcontactscount");
    }
}
