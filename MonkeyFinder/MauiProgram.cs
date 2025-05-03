using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MonkeyFinder.Services;
using MonkeyFinder.View;
#if ANDROID
using MonkeyFinder.Platforms.Android.Services;
#elif IOS
using MonkeyFinder.Platforms.iOS.Services;
#endif

namespace MonkeyFinder;

public static class MauiProgram
{
    /*
     * TODO: 1. Check and implement how to send messages
     * TODO: 2. Proper logging. There is Android.Util.Log, but what about iOS?
	 */

    public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

        // System services
        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
        builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
        builder.Services.AddSingleton<IMap>(Map.Default);

        // Services
#if ANDROID
        builder.Services.AddSingleton<IPermissionService, AndroidPermissionsService>();
        builder.Services.AddSingleton<IGeofencingService>(sp =>
        {
            //var context = Android.App.Application.Context;
            var context = MainApplication.Context;
            return new AndroidGeofencingService(context);

        });
        builder.Services.AddSingleton<INotificationService>(sp =>
        {
            //var context = Android.App.Application.Context;
            var context = MainApplication.Context;
            return new AndroidNotificationService(context);
        });
#elif IOS
        builder.Services.AddSingleton<IPermissionService, iOSPermissionsService>();
        builder.Services.AddSingleton<IGeofencingService, iOSGeofencingService>();
        builder.Services.AddSingleton<INotificationService, iOSNotificationService>();
#else
        builder.Services.AddSingleton<IGeofencingService, StubGeofencingService>();
#endif

        builder.Services.AddSingleton<MonkeyService>();

        // ViewModels
        builder.Services.AddSingleton<MonkeysViewModel>();
        builder.Services.AddTransient<MonkeyDetailsViewModel>();

        // Pages
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<DetailsPage>();

        return builder.Build();
	}
}
