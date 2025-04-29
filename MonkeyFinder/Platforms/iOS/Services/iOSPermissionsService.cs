using CoreLocation;
using MonkeyFinder.Services;

namespace MonkeyFinder.Platforms.iOS.Services
{
    public class iOSPermissionsService : IPermissionService
    {
        public async Task<bool> RequestLocationAlwaysAsync()
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
        public bool HasLocationAlwaysPermission()
        {
#pragma warning disable CA1422 // Validate platform compatibility
            return CLLocationManager.Status == CLAuthorizationStatus.AuthorizedAlways;
#pragma warning restore CA1422 // Validate platform compatibility
        }
    }
}
