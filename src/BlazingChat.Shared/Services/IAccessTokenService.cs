using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazingChat.Shared.Services
{
    public interface IAccessTokenService
    {
        Task<string> GetAccessTokenAsync(string tokenName);
        Task SetAccessTokenAsync(string tokenName, string tokenValue);
        Task RemoveAccessTokenAsync(string tokenName);
    }
}
