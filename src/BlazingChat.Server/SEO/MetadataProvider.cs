using System.Collections.Generic;

namespace BlazingChat.Server.SEO
{
    public class MetadataProvider
    {
        public Dictionary<string, MetadataValue> RouteDetailMapping { get; set; } = new()
        {
            {
                "/",
                new()
                {
                    Title = "BlazingChat - Login",
                    Description = "BlazingChat is a Blazor WebAssembly app developed by CuriousDrive for the community. This is a sample application for developers who are just getting started with Blazor."
                }
            },
            {
                "/about",
                new()
                {
                    Title = "BlazingChat - Profile",
                    Description = "This is BlazingChat's profile page."
                }
            }
        };
    }
}