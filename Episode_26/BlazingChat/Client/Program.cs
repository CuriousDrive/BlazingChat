using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using BlazingChat.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazingChat.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();

            //builder.Services.AddScoped(sp =>
            //    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddHttpClient("HostIS.ServerAPI",
                            client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
                            //.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                            .CreateClient("HostIS.ServerAPI"));

            builder.Services.AddApiAuthorization(options => {
                options.AuthenticationPaths.LogInPath = "security/login";
                options.AuthenticationPaths.LogInCallbackPath = "security/login-callback";
                options.AuthenticationPaths.LogInFailedPath = "security/login-failed";
                options.AuthenticationPaths.LogOutPath = "security/logout";
                options.AuthenticationPaths.LogOutCallbackPath = "security/logout-callback";
                options.AuthenticationPaths.LogOutFailedPath = "security/logout-failed";
                options.AuthenticationPaths.LogOutSucceededPath = "security/logged-out";
                options.AuthenticationPaths.ProfilePath = "security/profile";
                options.AuthenticationPaths.RegisterPath = "security/register";
            });

            builder.Services.AddHttpClient<IProfileViewModel, ProfileViewModel>
                ("BlazingChatClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

            builder.Services.AddHttpClient<IContactsViewModel, ContactsViewModel>
                ("BlazingChatClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

            builder.Services.AddHttpClient<ISettingsViewModel, SettingsViewModel>
                ("BlazingChatClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

            builder.Services.AddHttpClient<ILoginViewModel, LoginViewModel>
                ("BlazingChatClient", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

            await builder.Build().RunAsync();
        }
    }
}
