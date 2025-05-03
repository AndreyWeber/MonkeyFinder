using Android.App;
using MonkeyFinder.Services;
using Android.OS;
using AndroidApp = Android.App.Application;

namespace MonkeyFinder.Platforms.Android.Services
{
    public class AndroidPermissionsService : IPermissionService
    {
        public async Task<bool> RequestLocationAlwaysPermissionAsync()
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

        // TODO: Check if POST_NOTIFICATIONS granted by the system
        // TODO: Unified loggins (community libs)
        // TODO: Check if notifications muted by the user and if so, show a message or go to settings
        // TODO: Get better understanding of how permissions could be granted by the system and show permissions popup
        // TODO: Check if debugging tip: Toast.MakeText(context, message, ToastLength.Short).Show(); is needed
        // TODO: Understand what to do if Notification channels are not supported
        // TODO: Make sure how to use EnsureNotificationChannel() correctly

        public async Task<bool> RequestPostNotificationsPermissionAsync()
        {
            // Permission is not needed on Android < 33
            if (!OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                return true;
            }

            // Check permission current status
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            if (status == PermissionStatus.Granted)
            {
                // Permission already granted
                return true;
            }

            // Request the permission
            var requestedStatus = await Permissions.RequestAsync<Permissions.PostNotifications>();

            return requestedStatus == PermissionStatus.Granted;
        }
    }
}
