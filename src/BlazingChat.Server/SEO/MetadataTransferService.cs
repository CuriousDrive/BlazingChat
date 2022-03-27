using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazingChat.Server.SEO
{
    public class MetadataTransferService : INotifyPropertyChanged, IDisposable
    {
        private readonly NavigationManager _navigationManager;
        private readonly MetadataProvider _metadataProvider;
        public event PropertyChangedEventHandler PropertyChanged;

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

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        public MetadataTransferService(NavigationManager navigationManager, MetadataProvider metadataProvider)
        {
            _navigationManager = navigationManager;
            _metadataProvider = metadataProvider;

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
            var metadataValue = _metadataProvider.RouteDetailMapping.FirstOrDefault(vp => url.EndsWith(vp.Key)).Value;

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
        }

        public void Dispose()
        {
            // Unsubscribe the events
            _navigationManager.LocationChanged -= UpdateMetadata;
        }
    }
}