//using System.Net.Http;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazingChat.Shared.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazingChat.Client.Service
{
    public class UserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}
