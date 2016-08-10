using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AngularBBS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AngularBBS.Services
{
    public static class GithubConfig
    {
        public static string GetToken(IHttpContextAccessor httpContextAccessor)
        {
            var principal = httpContextAccessor.HttpContext.User;
            var token = principal.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                return token;
            }
            throw new Exception("Access token is not available");
        }

        public static string ClientId { get; set; }
        public static string Secret { get; set; }
        

        public const string BaseAddress = "https://github.com";
        public const string BaseApiAddress = "https://api.github.com";
        public const string AuthorizeEndpoint = BaseAddress + "/login/oauth/authorize";
        public const string TokenEndpoint = BaseAddress + "/login/oauth/access_token";
        public const string UserInfoEndPoint = BaseApiAddress + "/user";
        public const string ClaimIssure = "OAuth2-GithubConfig";
        public const string TokenClaimType = "access_token";
        public const string EmailClaimType = "urn:github:email";
        public const string UrlClaimType = "urn:github:url";
        public const string NameClaimType = "urn:github:name";

    }
    }

