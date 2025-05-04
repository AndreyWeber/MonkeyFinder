using MonkeyFinder.Services;
using UIKit;
using UserNotifications;
using Foundation;

namespace MonkeyFinder.Platforms.iOS.Services
{
    public class iOSNotificationService : INotificationService
    {
        public void SendGeofenceNotification(string title, string message, GeofenceTransition transtion)
        {
            var content = new UNMutableNotificationContent
            {
                Title = title,
                Body = message
            };

            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(0.1, false);
            var request = UNNotificationRequest.FromIdentifier(
                Guid.NewGuid().ToString(), content, trigger);
            UNUserNotificationCenter.Current.AddNotificationRequest(request, (error) =>
            {
                if (error != null)
                {
                    // TODO: Add logging
                    Console.WriteLine($"Error adding notification request: {error}");
                }
            });
        }
    }
}
