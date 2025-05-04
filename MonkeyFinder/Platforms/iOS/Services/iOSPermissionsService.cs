using CoreLocation;
using Foundation;
using MonkeyFinder.Services;
using UIKit;
using UserNotifications;

namespace MonkeyFinder.Platforms.iOS.Services
{
    public class iOSPermissionsService : IPermissionService
    {
        public async Task<bool> AreAppNotificationsEnabledAsync()
        {
            var settings = await UNUserNotificationCenter.Current
                .GetNotificationSettingsAsync();

            return settings.AuthorizationStatus is
                UNAuthorizationStatus.Authorized or
                UNAuthorizationStatus.Provisional or
                UNAuthorizationStatus.Ephemeral;
        }

        public void RequestAppNotificationSettings()
        {
            // iOS 15+: direct jump to settings
            if (OperatingSystem.IsIOSVersionAtLeast(15))
            {
                // Use the recommended overload for iOS 10.0 and later
                UNUserNotificationCenter.Current.Delegate?.OpenSettings(UNUserNotificationCenter.Current, null);
            }
            // iOS < 15: fall back to the app's Settings sheet
            else if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl(UIApplication.OpenSettingsUrlString)))
            {
                // Use the recommended overload for UIApplication.OpenUrl
                UIApplication.SharedApplication.OpenUrl(
                    new NSUrl(UIApplication.OpenSettingsUrlString),
                    new UIApplicationOpenUrlOptions(),
                    null
                );
            }
        }

        public async Task<bool> RequestLocationAlwaysPermissionAsync()
        {
            var manager = new CLLocationManager();
            var tcs = new TaskCompletionSource<bool>();

            manager.AuthorizationChanged += (sender, args) =>
            {
                if (args.Status == CLAuthorizationStatus.AuthorizedAlways)
                {
                    tcs.SetResult(true);
                }
                else if (manager.AuthorizationStatus == CLAuthorizationStatus.Denied ||
                    manager.AuthorizationStatus == CLAuthorizationStatus.Restricted)
                {
                    tcs.SetResult(false);
                }
            };

            manager.RequestAlwaysAuthorization();

            return await tcs.Task;
        }

        public Task<bool> RequestPostNotificationsPermissionAsync()
        {
            throw new NotImplementedException();
        }
    }
}
