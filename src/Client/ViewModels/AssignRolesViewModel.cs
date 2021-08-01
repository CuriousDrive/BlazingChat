using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BlazingChat.Shared.Models;
using Blazored.Toast.Services;

namespace BlazingChat.ViewModels
{
    public class AssignRolesViewModel : IAssignRolesViewModel
    {
        //properties
        public List<User> UsersWithoutRole { get; set; }

        private HttpClient _httpClient;
        public IToastService _toastService { get; }

        //methods
        public AssignRolesViewModel()
        {
        }
        public AssignRolesViewModel(HttpClient httpClient, IToastService toastService)
        {
            _httpClient = httpClient;
            _toastService = toastService;
        }

        public async Task LoadUsersWithoutRole()
        {
            UsersWithoutRole = await _httpClient.GetFromJsonAsync<List<User>>($"user/getuserswithoutrole");
        }
        public async Task AssignRole(long userId, string role)
        {
            var user = new User { UserId = userId, Role = role };
            await _httpClient.PutAsJsonAsync("user/assignrole", user);
        }

        public async Task DeleteUser(long userId)
        {
            await _httpClient.DeleteAsync("user/deleteuser/" + userId);
        }
    }
}
