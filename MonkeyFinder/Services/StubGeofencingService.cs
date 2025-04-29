using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyFinder.Services
{
    public class StubGeofencingService : IGeofencingService
    {
        public Task AddGeofencingAsync(double latitude, double longitude, float radiusMeters, string id)
        {
            throw new NotImplementedException();
        }
    }
}
