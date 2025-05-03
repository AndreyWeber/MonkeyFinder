namespace MonkeyFinder.Services
{
    public interface INotificationService
    {
        void SendGeofenceNotification(string title, string message, GeofenceTransition transtion);
    }
}
