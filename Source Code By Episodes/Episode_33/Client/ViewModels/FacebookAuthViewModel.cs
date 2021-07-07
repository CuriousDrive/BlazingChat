using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BlazingChat.Shared;
using BlazingChat.Shared.Models;

namespace BlazingChat.ViewModels
{
    public class FacebookAuthViewModel : IFacebookAuthViewModel
    {
        private readonly HttpClient _httpClient;
        public FacebookAuthViewModel(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        public async Task<string> GetFacebookJWTAsync(string accessToken)
        {
            var httpMessageResponse = await _httpClient.PostAsJsonAsync<FacebookAuthRequest>("user/getfacebookjwt", new FacebookAuthRequest() { AccessToken = accessToken });
            return (await httpMessageResponse.Content.ReadFromJsonAsync<AuthenticationResponse>()).Token;
        }
    }
}
