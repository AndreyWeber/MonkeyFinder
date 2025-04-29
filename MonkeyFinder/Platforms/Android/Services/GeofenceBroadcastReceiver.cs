using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Util;
using MonkeyFinder.Services;

namespace MonkeyFinder.Platforms.Android.Services
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter([AndroidGeofencingService.GeofenceBroadcastReceiverAction])]
    public class GeofenceBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (context is null)
            {
                Log.Error(nameof(GeofenceBroadcastReceiver), "Context is null");
                return;
            }

            if (intent is null)
            {
                Log.Error(nameof(GeofenceBroadcastReceiver), "Intent is null");
                return;
            }

            if (intent.Action != AndroidGeofencingService.GeofenceBroadcastReceiverAction)
            {
                Log.Error(nameof(GeofenceBroadcastReceiver), "Received unknown intent action: {0}", intent.Action ?? "null");
                return;
            }

            var geofencingEvent = GeofencingEvent.FromIntent(intent);
            if (geofencingEvent is null)
            {
                Log.Error(nameof(GeofenceBroadcastReceiver), "Geofencing event is null");
                return;
            }

            if (geofencingEvent.HasError)
            {
                var errorMessage = GeofenceStatusCodes.GetStatusCodeString(geofencingEvent.ErrorCode);
                Log.Error(nameof(GeofenceBroadcastReceiver), "Error in geofencing event: {0}", errorMessage);
                return;
            }

            // Updated to use IPlatformApplication.Current.Services
            var notificationService = IPlatformApplication.Current?.Services.GetService<INotificationService>();
            if (notificationService == null)
            {
                Log.Error(nameof(GeofenceBroadcastReceiver), "Notification service is not available");
                return;
            }

            var transitionType = geofencingEvent.GeofenceTransition;
            switch (transitionType)
            {
                case Geofence.GeofenceTransitionEnter:
                    Log.Debug(nameof(GeofenceBroadcastReceiver), "Entering geofence area");
                    notificationService.SendGeofenceNotification("Entering Geofence", "You have entered the geofence area.");
                    break;
                case Geofence.GeofenceTransitionExit:
                    Log.Debug(nameof(GeofenceBroadcastReceiver), "Exiting geofence area");
                    notificationService.SendGeofenceNotification("Exiting Geofence", "You have exited the geofence area.");
                    break;
                default:
                    Log.Error(nameof(GeofenceBroadcastReceiver), "Unknown transition type: {0}", transitionType);
                    break;
            }
        }
    }
}
