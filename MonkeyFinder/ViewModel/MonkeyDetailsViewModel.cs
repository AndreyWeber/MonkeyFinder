using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyFinder.ViewModel
{
    [QueryProperty("Monkey", "Monkey")]
    public partial class MonkeyDetailsViewModel : BaseViewModel
    {
        private IMap _map;

        [ObservableProperty]
        private Monkey? _monkey;

        public MonkeyDetailsViewModel(IMap map)
        {
            this._map = map;
        }

        [RelayCommand]
        private async Task OpenMapAsync()
        {
            try
            {
                var placemark = new Placemark
                {
                    Location = new Location(Monkey.Latitude, Monkey.Longitude),
                };

                await _map.OpenAsync(placemark,
                    new MapLaunchOptions
                    {
                        Name = Monkey.Name,
                        NavigationMode = NavigationMode.None
                    });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error", $"Unable to open map: {ex.Message}", "OK");
            }
        }
    }
}
