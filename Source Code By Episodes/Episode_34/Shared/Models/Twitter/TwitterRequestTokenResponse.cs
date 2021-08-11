namespace BlazingChat.Shared
{
    public class TwitterRequestTokenResponse
    {
        public string OAuthToken { get; set; }
        public string OAuthTokenSecrete { get; set; }
        public string OAuthCallBackConfirmed { get; set; }
        public string OAuthVerifier { get; set; }

    }
}