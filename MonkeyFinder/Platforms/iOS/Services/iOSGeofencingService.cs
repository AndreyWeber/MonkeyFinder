using CoreLocation;
using MonkeyFinder.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyFinder.Platforms.iOS.Services
{
    public class iOSGeofencingService : IGeofencingService
    {
        private readonly CLLocationManager _locationManager;

        public iOSGeofencingService()
        {
            _locationManager = new CLLocationManager
            {
                Delegate = new GeofenceLocationDelegate()
            };
        }
        public Task AddGeofencingAsync(double latitude, double longitude, float radiusMeters, string id)
        {
            _locationManager.RequestAlwaysAuthorization();

#pragma warning disable CA1416 // Validate platform compatibility
            var region = new CLCircularRegion(new CLLocationCoordinate2D(latitude, longitude), radiusMeters, id)
            {
                NotifyOnEntry = true,
                NotifyOnExit = true
            };
#pragma warning restore CA1416 // Validate platform compatibility

#pragma warning disable CA1422 // Validate platform compatibility
            _locationManager.StartMonitoring(region);
#pragma warning restore CA1422 // Validate platform compatibility

            return Task.CompletedTask;
        }
    }
}
