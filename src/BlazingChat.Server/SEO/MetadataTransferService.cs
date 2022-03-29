using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazingChat.Server.SEO
{
    public class MetadataTransferService : INotifyPropertyChanged, IDisposable
    {
        private readonly NavigationManager _navigationManager;
        public event PropertyChangedEventHandler PropertyChanged;
        private List<MetadataValue> MetadataValues {get; set;}

        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _description;

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private string _ogTitle;

        public string OgTitle
        {
            get => _ogTitle;
            set
            {
                _ogTitle = value;
                OnPropertyChanged();
            }
        }

        private string _twitterCard;

        public string TwitterCard
        {
            get => _twitterCard;
            set
            {
                _twitterCard = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        public MetadataTransferService(NavigationManager navigationManager)
        {
            LoadMetadataValues();
            _navigationManager = navigationManager;

            // Susbscribe to the location changed event
            _navigationManager.LocationChanged += UpdateMetadata;

            // Initial Call
            UpdateMetadata(_navigationManager.Uri);
        }
        private void UpdateMetadata(object sender, LocationChangedEventArgs e)
        {
            UpdateMetadata(e.Location);
        }
        private void UpdateMetadata(string url)
        {
            var metadataValue = MetadataValues.FirstOrDefault(mv => url.EndsWith(mv.Url));

            if (metadataValue is null)
            {
                metadataValue = new()
                {
                    Title = "Default",
                    Description = "Default"
                };
            }

            Title = metadataValue.Title;
            Description = metadataValue.Description;
            OgTitle = metadataValue.OgTitle;
            TwitterCard = metadataValue.TwitterCard;
        }
        private void LoadMetadataValues()
        {
            MetadataValues = new List<MetadataValue>();

            MetadataValues.Add(
                new()
                {
                    Url = "/",
                    Title = "BlazingChat - Login",
                    Description = "BlazingChat is a Blazor WebAssembly app developed by CuriousDrive for the community. This is a sample application for developers who are just getting started with Blazor."
                });

            MetadataValues.Add(
                new()
                {
                    Url = "/profile",
                    Title = "BlazingChat - Profile",
                    Description = "This is a profile page for BlazingChat user."
                });

            MetadataValues.Add(
                new()
                {
                    Url = "/contacts",
                    Title = "BlazingChat - Contact",
                    Description = "This is a contacts page for BlazingChat user."
                });

            MetadataValues.Add(
                new()
                {
                    Url = "/settings",
                    Title = "BlazingChat - Settings",
                    Description = "This is a settings page for BlazingChat user."
                });
        }
    
        public void Dispose()
        {
            // Unsubscribe the events
            _navigationManager.LocationChanged -= UpdateMetadata;
        }
    }

    public class MetadataValue
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string OgTitle { get; set; } = "This title is for Facebook";
        public string TwitterCard { get; set; } = "This Title is for Twitter";
    }
}