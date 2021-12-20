using System;
using BlazingChat.Client;
using BlazingChat.Shared.Extensions;
using BlazingChat.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

#region ConfigureServices

var applicationSettingsSection = builder.Configuration.GetSection("ApplicationSettings");

builder.Services.Configure<ApplicationSettings>(options =>
{
    applicationSettingsSection.Bind(options);
});

builder.Services.AddBlazingChat(applicationSettingsSection.Get<ApplicationSettings>());

#endregion

await builder.Build().RunAsync();
