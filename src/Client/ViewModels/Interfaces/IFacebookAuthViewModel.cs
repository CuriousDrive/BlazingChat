using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using BlazingChat.Shared.Models;

namespace BlazingChat.ViewModels
{
    public interface IFacebookAuthViewModel
    {
        public Task<string> GetFacebookJWTAsync(string accessToken);
    }
}
