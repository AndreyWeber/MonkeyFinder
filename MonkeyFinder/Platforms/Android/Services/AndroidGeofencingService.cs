using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Util;
using Android.Gms.Extensions;
using MonkeyFinder.Services;
using Android.Gms.Common.Apis;
using GmsTasks = Android.Gms.Tasks;
using Android.OS;


namespace MonkeyFinder.Platforms.Android.Services
{
    public class AndroidGeofencingService : Java.Lang.Object, GmsTasks.IOnCompleteListener, IGeofencingService
    {
        public const string GeofenceBroadcastReceiverAction = "com.companyname.monkeyfinder.ACTION_GEOFENCE_EVENT";

        private readonly IGeofencingClient _geofencingClient;
        private readonly Context _context;

        private PendingIntent? _geofencePendingIntent = null;

        public AndroidGeofencingService(Context context)
        {
            _context = context;
            _geofencingClient = LocationServices.GetGeofencingClient(_context);
        }

        public async Task AddGeofencingAsync(double latitude, double longitude, float radiusMeters, string id)
        {
            var geofence = new GeofenceBuilder()
                .SetRequestId(id)
                .SetCircularRegion(latitude, longitude, radiusMeters)
                .SetExpirationDuration(Geofence.NeverExpire)
                .SetTransitionTypes(Geofence.GeofenceTransitionEnter | Geofence.GeofenceTransitionExit)
                .Build();

            var geofencingRequest = new GeofencingRequest.Builder()
                .SetInitialTrigger(GeofencingRequest.InitialTriggerEnter)
                .AddGeofence(geofence)
                .Build();

            try
            {
                var pendingIntent = CreateGeofencePendingIntent();
                if (pendingIntent is null)
                {
                    Log.Error(nameof(AndroidGeofencingService), "Pending intent is null");
                    return;
                }

                // NOTE: adb shell dumpsys activity service com.google.android.gms/com.google.android.location.internal.GoogleLocationManagerService

                // TODO: 1. Notifications for geofence transitions
                // TODO: 2. Check possibility of using of Shiny.Notifications and or Plugin.LocalNotification
                // TODO: 3. How to pick some payload from intent when geofence is triggered?
                // TODO: 4. Logging

                await _geofencingClient
                    .AddGeofences(geofencingRequest, pendingIntent)
                    .AddOnCompleteListener(this);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(AndroidGeofencingService), "Failed when adding geofence: {0}", ex.Message);
            }
        }

        private PendingIntent? CreateGeofencePendingIntent()
        {
            if (_geofencePendingIntent is not null)
            {
                return _geofencePendingIntent;
            }

            var intent = new Intent(_context, typeof(GeofenceBroadcastReceiver))
                .SetAction(GeofenceBroadcastReceiverAction);

#pragma warning disable CA1416 // Validate platform compatibility
            var flags = Build.VERSION.SdkInt >= BuildVersionCodes.S
                ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Mutable
                : PendingIntentFlags.UpdateCurrent;
#pragma warning restore CA1416 // Validate platform compatibility

            _geofencePendingIntent = PendingIntent.GetBroadcast(_context, requestCode: 0, intent, flags);
            //_geofencePendingIntent = PendingIntent.GetService(_context, requestCode: 0, intent, flags);

            return _geofencePendingIntent;
        }

        public void OnComplete(GmsTasks.Task task)
        {
            if (task.IsSuccessful)
            {
                Log.Info(nameof(AndroidGeofencingService), "Geofence added successfully.");
            }

            if (task.Exception is ApiException apiEx)
            {
                var code = apiEx.StatusCode;
                Log.Error(nameof(AndroidGeofencingService), $"Failed to add geofence: {0} {1}",
                    code, GeofenceStatusCodes.GetStatusCodeString(code));
            }
        }
    }
}
