using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularBBS.Services
{
    public static class AccessToken
    {
        public static string Token { get; set; }
        public static TimeSpan? Expiry { get; set; }
    }
}
