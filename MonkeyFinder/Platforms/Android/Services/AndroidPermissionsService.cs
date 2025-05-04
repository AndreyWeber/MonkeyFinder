using MonkeyFinder.Services;
using Android.OS;
using Android.Provider;
using Android.Content;
using AndroidX.Core.App;

using AndroidNet = Android.Net;

namespace MonkeyFinder.Platforms.Android.Services
{
    public class AndroidPermissionsService(Context context) : IPermissionService
    {
        private readonly Context _context = context;

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

        // TODO: Unified logging (community libs)
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

        public Task<bool> AreAppNotificationsEnabledAsync()
        {
            var enabled = NotificationManagerCompat
                .From(_context)
                .AreNotificationsEnabled();
            return Task.FromResult(enabled);
        }

        public void RequestAppNotificationSettings()
        {
            Intent intent;

            // API-26 (Oreo) and above - directly open app settings
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
#pragma warning disable CA1416 // Validate platform compatibility
                intent = new Intent(Settings.ActionAppNotificationSettings)
                .AddFlags(ActivityFlags.NewTask)
                    .PutExtra(Settings.ExtraAppPackage, _context.PackageName);
#pragma warning restore CA1416 // Validate platform compatibility
            }
            // Older versions - fallback to the general app settings
            else
            {
                intent = new Intent(Settings.ActionApplicationDetailsSettings)
                .AddFlags(ActivityFlags.NewTask)
                    .SetData(AndroidNet.Uri.Parse($"package:{_context.PackageName}"));
            }

            _context.StartActivity(intent);
        }
    }
}
