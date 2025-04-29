using MonkeyFinder.Services;
using MonkeyFinder.View;

namespace MonkeyFinder.ViewModel
{
    public partial class MonkeysViewModel : BaseViewModel
    {
        private readonly MonkeyService _monkeyService;
        private readonly IConnectivity _connectivity;
        private readonly IGeolocation _geolocation;
        private readonly IPermissionService _permissionService;
        private readonly IGeofencingService _geofencingService;

        public ObservableCollection<Monkey> Monkeys { get; } = [];

        public MonkeysViewModel(
            MonkeyService monkeyService,
            IConnectivity connectivity,
            IGeolocation geolocation,
            IPermissionService permissionService,
            IGeofencingService geofencingService)
        {
            _monkeyService = monkeyService;
            _connectivity = connectivity;
            _geolocation = geolocation;
            _permissionService = permissionService;
            _geofencingService = geofencingService;

            Title = "Monkey Finder";
        }

        [RelayCommand]
        private async Task GetClosestMonkeyAsync()
        {
            if (IsBusy || Monkeys.Count == 0)
            {
                return;
            }

            try
            {
                IsBusy = true;

                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status != PermissionStatus.Granted)
                {
                    Debug.WriteLine("Location permission denied.");
                    return;
                }

                var location = await _geolocation.GetLastKnownLocationAsync();
                if (location is null)
                {
                    location = await _geolocation.GetLocationAsync(new GeolocationRequest
                    {
                        DesiredAccuracy = GeolocationAccuracy.Medium,
                        Timeout = TimeSpan.FromSeconds(30)
                    });
                }

                if (location is null)
                {
                    return;
                }

                var firstMonkey = Monkeys
                    .OrderBy(m => new Location(m.Latitude, m.Longitude)
                        .CalculateDistance(location.Latitude, location.Longitude, DistanceUnits.Miles)
                    )
                    .FirstOrDefault();

                if (firstMonkey is null)
                {
                    return;
                }

                await Shell.Current.DisplayAlert("Closest Monkey", $"The closest monkey is {firstMonkey.Name} at {firstMonkey.Location}", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get closest monkey: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Unable to get closest monkey: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }

        }

        [RelayCommand]
        private async Task GoToDetailsAsync(Monkey monkey)
        {
            if (monkey is null)
            {
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(DetailsPage)}", true,
                new Dictionary<string, object>
                {
                    { "Monkey", monkey }
                });
        }

        [RelayCommand]
        private async Task GetMonkeysAsync()
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await Shell.Current.DisplayAlert("No Internet", "No internet connection available.", "OK");
                    return;
                }

                IsBusy = true;

                var monkeys = (await _monkeyService.GetMonkeysAsync()).ToList();
                if (monkeys.Count > 0)
                {
                    Monkeys.Clear();
                }

                var baboon = monkeys.FirstOrDefault(m => m.Name == "Baboon");
                //foreach (var monkey in monkeys)
                if (baboon != null)
                {
                    await _geofencingService.AddGeofencingAsync(baboon.Latitude, baboon.Longitude, 300.00f, baboon.Name);
                    //await _geofencingService.AddGeofencingAsync(monkey.Latitude, monkey.Longitude, 100.00f, monkey.Name);
                    Monkeys.Add(baboon);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get monkeys: {ex.Message}");
                //await Shell.Current.DisplayAlert("Error", $"Unable to get monkeys: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task EnableLocationFeaturesAsync()
        {
            var granted = await _permissionService.RequestLocationAlwaysAsync();
            if (!granted)
            {
                await Shell.Current.DisplayAlert("Permission Denied", "Location permission is required to use this feature.", "OK");
                return;
            }
        }
    }
}