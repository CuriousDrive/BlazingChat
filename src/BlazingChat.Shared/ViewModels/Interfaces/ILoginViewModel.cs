using BlazingChat.Shared;
using BlazingChat.Shared.Models;
using System.Threading.Tasks;

namespace BlazingChat.ViewModels
{
    public interface ILoginViewModel
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public Task LoginUser();
        public Task<AuthenticationResponse> AuthenticateJWT();
        public Task<string> GetFacebookAppIDAndRedirectUriAsync();
        public Task<TwitterRequestTokenResponse> GetTwitterOAuthTokenAsync();
        public Task<string> GetGoogleClientIDAndRedirectUriAsync();
        public Task<User> GetUserByJWTAsync(string jwtToken);
        public Task<string> GetTwitterJWTAsync(TwitterRequestTokenResponse twitterRequestTokenResponse);
        public Task<string> GetFacebookJWTAsync(string accessToken);
    }
}