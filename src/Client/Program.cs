using BlazingChat.Client;
using BlazingChat.Client.Handlers;
using BlazingChat.Client.Logging;
using BlazingChat.ViewModels;
using Blazored.LocalStorage;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

#region ConfigureServices

string baseAddress = builder.Configuration["BaseAddress"];

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });

builder.Services.ConfigureAll<HttpClientFactoryOptions>(options =>
    options.HttpMessageHandlerBuilderActions.Add(handlerBuilder =>
        handlerBuilder.AdditionalHandlers.Add(
            handlerBuilder.Services.GetRequiredService<CustomAuthorizationHandler>())));

//transactional named http clients
builder.Services.AddHttpClient<IProfileViewModel, ProfileViewModel>
    ("ProfileViewModelClient", client => client.BaseAddress = new Uri(baseAddress));

builder.Services.AddHttpClient<IContactsViewModel, ContactsViewModel>
    ("ContactsViewModelClient", client => client.BaseAddress = new Uri(baseAddress));

builder.Services.AddHttpClient<ISettingsViewModel, SettingsViewModel>
    ("SettingsViewModelClient", client => client.BaseAddress = new Uri(baseAddress));

builder.Services.AddHttpClient<IAssignRolesViewModel, AssignRolesViewModel>
    ("AssignRolesViewModel", client => client.BaseAddress = new Uri(baseAddress));

//authentication http clients
builder.Services.AddHttpClient<ILoginViewModel, LoginViewModel>
    ("LoginViewModelClient", client => client.BaseAddress = new Uri(baseAddress));

builder.Services.AddHttpClient<IRegisterViewModel, RegisterViewModel>
    ("RegisterViewModelClient", client => client.BaseAddress = new Uri(baseAddress));

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddTransient<CustomAuthorizationHandler>();
builder.Services.AddLogging(logging =>
{
    var httpClient = builder.Services.BuildServiceProvider().GetRequiredService<HttpClient>();
    var authenticationStateProvider = builder.Services.BuildServiceProvider().GetRequiredService<AuthenticationStateProvider>();
    logging.SetMinimumLevel(LogLevel.Error);
    logging.AddProvider(new ApplicationLoggerProvider(httpClient, authenticationStateProvider));
});
builder.Services.AddBlazoredToast();

#endregion

await builder.Build().RunAsync();
