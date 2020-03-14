using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace BlazingChat.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            
            // added this with Blazor 3.2 preview 2
            builder.Services.AddBaseAddressHttpClient();
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            
            builder.Services.AddScoped<AuthenticationStateProvider,CustomAuthenticationStateProvider>();        
            
            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}
