using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngularBBS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AngularBBS.Services
{
    public class TokenServices
    {
        public string GetToken()
        {
            if (!string.IsNullOrEmpty(AccessToken.Token))
            {
                return AccessToken.Token;
            }

            throw new Exception("Access token is not available");
        }
    }
}
