using Android.OS;
using Android.App;
using Android.Content;
using MonkeyFinder.Services;
using AndroidX.Core.App;

namespace MonkeyFinder.Platforms.Android.Services
{
    public class AndroidNotificationService(Context context) : INotificationService
    {
        public const string ChannelId = "monkey_finder_geofence_alert";

        private static int currentNotificationId = 0;

        private readonly Context _context = context;

        public void SendGeofenceNotification(string title, string message, GeofenceTransition transition)
        {
            EnsureNotificationChannel();

            var builder = new NotificationCompat.Builder(_context, ChannelId)
                .SetSmallIcon(GetGeofenceNotificationIcon(transition))
                .SetContentTitle(title)
                .SetContentText(message)
                .SetAutoCancel(true)
                .SetPriority((int)NotificationPriority.High)
                .SetDefaults((int)NotificationDefaults.All);

            NotificationManagerCompat
                .From(_context)
                .Notify(GetNextNotificationId(), builder.Build());
        }

        private void EnsureNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are not supported on this version of Android
                // TODO: Add logging
                return;
            }

            var manager = (NotificationManager)_context.GetSystemService(Context.NotificationService)!;

#pragma warning disable CA1416 // Validate platform compatibility
            if (manager.GetNotificationChannel(ChannelId) != null)
            {
                // Notification channel already exists
                return;
            }

            var channel = new NotificationChannel(
                ChannelId,
                "Geofence Notifications",
                NotificationImportance.High)
            {
                Description = "Notifications for geofence transitions"
            };

            // TODO: Do I need to set these properties?
            channel.EnableVibration(true);
            channel.EnableLights(true);

            manager.CreateNotificationChannel(channel);
#pragma warning restore CA1416 // Validate platform compatibility
        }

        private static int GetNextNotificationId() =>
            Interlocked.Increment(ref currentNotificationId);

        // TODO: Add suport of Dwell transition. Dwell icon is needed
        private static int GetGeofenceNotificationIcon(GeofenceTransition transition) => transition switch
        {
            GeofenceTransition.Enter => Resource.Drawable.entering_geo_fence,
            GeofenceTransition.Exit => Resource.Drawable.leaving_geo_fence,
            _ => throw new ArgumentOutOfRangeException(nameof(transition), transition, "Unsupported or unknown GeofenceTransition type")
        };
    }
}