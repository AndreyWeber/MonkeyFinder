using CoreLocation;
using MonkeyFinder.Services;

namespace MonkeyFinder.Platforms.iOS.Services
{
    public class iOSPermissionsService : IPermissionService
    {
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
