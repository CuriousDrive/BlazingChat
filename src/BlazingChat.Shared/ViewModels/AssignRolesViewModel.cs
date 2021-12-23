using BlazingChat.Shared.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazingChat.ViewModels
{
    public class AssignRolesViewModel : IAssignRolesViewModel
    {
        public IEnumerable<User> UsersWithoutRole { get; private set; } = new List<User>();

        private readonly HttpClient _httpClient;

        public AssignRolesViewModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task LoadUsersWithoutRole() =>
            UsersWithoutRole = await _httpClient.GetFromJsonAsync<List<User>>("user/getuserswithoutrole");

        public Task AssignRole(long userId, string role) =>
            _httpClient.PutAsJsonAsync("user/assignrole", new User { UserId = userId, Role = role });

        public Task DeleteUser(long userId) =>
            _httpClient.DeleteAsync($"user/deleteuser/{userId}");
    }
}
