using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularBBS.Services
{
    public class GithubConfigService
    {
        public string secret {
            get {return Environment.GetEnvironmentVariable("GITHUB_SECRET");}
        }
        public string clientId = "3c6f609063f565fb2ed7";
    }
}
