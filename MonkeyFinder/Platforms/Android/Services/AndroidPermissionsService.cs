using Android;
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Util;
using Android.Content.PM;
using MonkeyFinder.Services;
using System.Threading.Tasks;

namespace MonkeyFinder.Platforms.Android.Services
{
    public class AndroidPermissionsService : IPermissionService
    {
        public async Task<bool> RequestLocationAlwaysAsync()
        {
            // 1. Check if Fine (Foreground) the permission is granted
            var fineStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (fineStatus != PermissionStatus.Granted)
            {
                // Request the foreground location permission
                fineStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (fineStatus != PermissionStatus.Granted)
                {
                    // Permission denied
                    return false;
                }
            }

            // 2. On Android 10+ (API 29+), background location is separate
            if (OperatingSystem.IsAndroidVersionAtLeast(29))
            {
                // Check if background is already granted
                var backgroundStatus = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();
                if (backgroundStatus != PermissionStatus.Granted)
                {
                    // Request the background location permission
                    backgroundStatus = await Permissions.RequestAsync<Permissions.LocationAlways>();
                    if (backgroundStatus != PermissionStatus.Granted)
                    {
                        // Permission denied
                        return false;
                    }
                }
            }

            // If we get here, we have (at least) the best location permission available
            return true;
        }

        public bool HasLocationAlwaysPermission()
        {
            // On Android < 29, there's effectively no separate "background" permission.
            // The user either has Fine location (which covers it all).
            // On Android >= 29, you must check both File + Background.

#pragma warning disable CA1416 // Validate platform compatibility
            var finePermission = Platform.CurrentActivity?.CheckSelfPermission(Manifest.Permission.AccessFineLocation);
#pragma warning restore CA1416 // Validate platform compatibility
            if (finePermission != Permission.Granted)
            {
                return false;
            }

            if (OperatingSystem.IsAndroidVersionAtLeast(29))
            {
                var backgroundPermission = Platform.CurrentActivity?.CheckSelfPermission(Manifest.Permission.AccessBackgroundLocation);
                if (backgroundPermission != Permission.Granted)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
