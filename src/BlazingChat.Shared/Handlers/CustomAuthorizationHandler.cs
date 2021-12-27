using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace BlazingChat.Client.Handlers
{
    public class CustomAuthorizationHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly ISyncLocalStorageService _syncLocalStorageService;

        public CustomAuthorizationHandler(ILocalStorageService localStorageService,
            ISyncLocalStorageService syncLocalStorageService)
        {
            //injecting local storage service
            _localStorageService = localStorageService;
            _syncLocalStorageService = syncLocalStorageService;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //getting token from the localstorage
            string jwtToken;
            try
            {
                jwtToken = await _localStorageService.GetItemAsync<string>("jwt_token");
            }
            catch
            {
                jwtToken = _syncLocalStorageService.GetItem<string>("jwt_token");
            }

            //adding the token in authorization header
            if (jwtToken != null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            //sending the request
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
