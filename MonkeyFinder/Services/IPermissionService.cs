using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonkeyFinder.Services
{
    public interface IPermissionService
    {
        Task<bool> RequestLocationAlwaysAsync();
        bool HasLocationAlwaysPermission();
    }
}
