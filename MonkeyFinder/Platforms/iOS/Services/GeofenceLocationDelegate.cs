using CoreLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyFinder.Platforms.iOS.Services
{
    public class GeofenceLocationDelegate : CLLocationManagerDelegate
    {
        // Add common implementation of a class handling geofence events

        public override void RegionEntered(CLLocationManager manager, CLRegion region)
        {
            base.RegionEntered(manager, region);

            //Log.Debug(nameof(GeofenceLocationDelegate), "Entering geofence area");
        }

        public override void RegionLeft(CLLocationManager manager, CLRegion region)
        {
            base.RegionLeft(manager, region);

            //Log.Debug(nameof(GeofenceLocationDelegate), "Exiting geofence area");
        }
    }
}
