using Android.OS;
using Android.App;
using Android.Content;
using MonkeyFinder.Services;
using AndroidX.Core.App;

namespace MonkeyFinder.Platforms.Android.Services
{
    public class AndroidNotificationService : INotificationService
    {
        public const string ChannelId = "monkey_finder_geofence_alert";

        private static int currentNotificationId = 0;
        
        private readonly Context _context;

        public AndroidNotificationService(Context context)
        {
            _context = context;
        }

        public void SendGeofenceNotification(string title, string message)
        {
            EnsureNotificationChannel();

            var builder = new NotificationCompat.Builder(_context, ChannelId)
                .SetSmallIcon(Resource.Drawable.material_ic_menu_arrow_up_black_24dp)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetAutoCancel(true)
                .SetPriority((int)NotificationPriority.High)
                .SetPriority((int)NotificationDefaults.All);

            NotificationManagerCompat.From(_context)
                .Notify(GetNextNotificationId(), builder.Build());
        }

        private void EnsureNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are not supported on this version of Android
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
                "Geofence Alerts",
                NotificationImportance.High)
            {
                Description = "Notifications for geofence transitions"
            };
#pragma warning restore CA1416 // Validate platform compatibility
        }

        private static int GetNextNotificationId() =>
            Interlocked.Increment(ref currentNotificationId);
    }
}
