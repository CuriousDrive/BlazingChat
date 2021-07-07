using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BlazingChat.Shared;
using BlazingChat.Shared.Models;

namespace BlazingChat.ViewModels
{
    public interface ILoginViewModel
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public Task LoginUser();
        public Task<AuthenticationResponse> AuthenticateJWT();
        public Task<string> GetFacebookAppIDAsync();
        public Task<User> GetUserByJWTAsync(string jwtToken);
    }
}