using System.Net.Http;
using BlazingChat.Shared.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazingChat.ViewModels
{
    public class ProfileViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Message { get; set; }

        private HttpClient _httpClient { get; set; }

        public ProfileViewModel(HttpClient httpclient)
        {
            this._httpClient = httpclient;
        }

        public async Task UpdateProfile()
        {
            User user = this;
            user.UserId = 10;
            user.Password = user.EmailAddress;
            user.Source = "APPL";

            string serializedUser = JsonSerializer.Serialize(user);

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, "user/updateprofile/10");
            requestMessage.Content = new StringContent(serializedUser);

            requestMessage.Content.Headers.ContentType
                = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(requestMessage);

            var responseStatusCode = response.StatusCode;
            var responseBody = await response.Content.ReadAsStringAsync();

            user = JsonSerializer.Deserialize<User>(responseBody);

            Message = "Records updated succesfully";
        }

        public static implicit operator User(ProfileViewModel profileViewModel)
        {
            return new User
            {
                FirstName = profileViewModel.FirstName,
                LastName = profileViewModel.LastName,
                EmailAddress = profileViewModel.EmailAddress
            };
        }
    }
}