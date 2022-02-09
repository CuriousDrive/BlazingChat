using BlazingChat.Shared.Extensions;
using BlazingChat.Shared.Models;
using BlazingChat.Shared.Services;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazingChat.ViewModels
{
    public class AssignRolesViewModel : IAssignRolesViewModel
    {
        public IEnumerable<User> AllUsers { get; private set; } = new List<User>();

        private readonly HttpClient _httpClient;
        private readonly IAccessTokenService _accessTokenService;

        public AssignRolesViewModel(HttpClient httpClient,
            IAccessTokenService accessTokenService)
        {
            _httpClient = httpClient;
            _accessTokenService = accessTokenService;
        }

        public async Task LoadAllUsers()
        {
            var jwtToken = await _accessTokenService.GetAccessTokenAsync("jwt_token");
            AllUsers = await _httpClient.GetAsync<List<User>>("user/getallusers", jwtToken);
        }

        public async Task AssignRole(long userId, string role)
        {
            var jwtToken = await _accessTokenService.GetAccessTokenAsync("jwt_token");
            await _httpClient.PutAsync<int>("user/assignrole", new User { UserId = userId, Role = role }, jwtToken);
        }

        public async Task DeleteUser(long userId)
        {
            var jwtToken = await _accessTokenService.GetAccessTokenAsync("jwt_token");
            int result = await _httpClient.DeleteAsync($"user/deleteuser/{userId}", jwtToken);
        }
    }
}
