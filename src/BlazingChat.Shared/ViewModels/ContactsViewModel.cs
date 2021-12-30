using BlazingChat.Shared.Extensions;
using BlazingChat.Shared.Models;
using BlazingChat.Shared.Services;
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
        private readonly IAccessTokenService _accessTokenService;

        public ContactsViewModel(HttpClient httpClient,
            IAccessTokenService accessTokenService)
        {
            _httpClient = httpClient;
            _accessTokenService = accessTokenService;
        }

        private void LoadCurrentObject(IEnumerable<User> users) =>
            Contacts = users.Select(u => (Contact)u);

        //loading actual contacts
        public async Task LoadAllContactsDB()
        {
            var jwtToken = await _accessTokenService.GetAccessTokenAsync("jwt_token");
            var users = await _httpClient.GetAsync<List<User>>("contacts/getcontacts", jwtToken);
            LoadCurrentObject(users);
        }

        public async Task LoadOnlyVisibleContactsDB(int startIndex, int count)
        {
            var jwtToken = await _accessTokenService.GetAccessTokenAsync("jwt_token");
            var users = await _httpClient.GetAsync<List<User>>($"contacts/getvisiblecontacts?startIndex={startIndex}&count={count}", jwtToken);
            LoadCurrentObject(users);
        }

        public async Task LoadContactsCountDB()
        {
            var jwtToken = await _accessTokenService.GetAccessTokenAsync("jwt_token");
            ContactsCount = await _httpClient.GetAsync<int>("contacts/getcontactscount", jwtToken);
        }
    }
}
