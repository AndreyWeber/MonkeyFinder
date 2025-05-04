using System;
namespace MonkeyFinder.Services
{
    public interface IPermissionService
    {
        Task<bool> RequestLocationAlwaysPermissionAsync();
        Task<bool> RequestPostNotificationsPermissionAsync();

        Task<bool> AreAppNotificationsEnabledAsync();
        void RequestAppNotificationSettings();
    }
}
