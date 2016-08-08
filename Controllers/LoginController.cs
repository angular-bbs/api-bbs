using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AngularBBS.Services;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.v3;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AngularBBS.Controllers
{
    [Route("api/login-from-github")]
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;

        public LoginController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET api/values
        [HttpGet]
        public async Task GetAsync()
        {
            var url = "https://api.github.com/authorizations/clients/" + GitHubSecret.ClientId;
            var tokenRequestParameters = new Dictionary<string, string>()
            {
                { "client_secret", GitHubSecret.Secret}
            };

            var requestContent = new FormUrlEncodedContent(tokenRequestParameters);
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, url);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken.Token);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Headers.Add("User-Agent", "Angular-bbs");
            requestMessage.Content = requestContent;


            var response = await _httpClient.SendAsync(requestMessage, HttpContext.RequestAborted);
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                return;
            }
            else
            {
                var error = "OAuth token endpoint failure: " + await Display(response);
                return;
            }
            //            await HttpContext.Authentication.ChallengeAsync("GitHub");
//            var url = "https://api.github.com/user";// + GitHubSecret.ClientId;
//
//            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
//            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken.Token);
//            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//            requestMessage.Headers.Add("User-Agent", "Angular-bbs");
//
//
//            var response = await _httpClient.SendAsync(requestMessage, HttpContext.RequestAborted);
//            response.EnsureSuccessStatusCode();
//            if (response.IsSuccessStatusCode)
//            {
//                var user = JObject.Parse(await response.Content.ReadAsStringAsync());
//                return;
//            }
//            else
//            {
//                var error = "OAuth token endpoint failure: " + await Display(response);
//                return;
//            }
//            return;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public object Post()
        {
            return new {name = "superman"};
        }

        // PUT api/values/5
        [HttpPut("")]
        public async Task PutAsync()
        {
            var url = "https://api.github.com/authorizations/clients/" + GitHubSecret.ClientId;
            var tokenRequestParameters = new Dictionary<string, string>()
            {
                { "client_secret", GitHubSecret.Secret}
            };

            var requestContent = new FormUrlEncodedContent(tokenRequestParameters);

            var requestMessage = new HttpRequestMessage(HttpMethod.Put, url);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Headers.Add("User-Agent", "Angular-bbs");
            requestMessage.Content = requestContent;

            var response = await _httpClient.SendAsync(requestMessage, HttpContext.RequestAborted);
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
                return;
            }
            else
            {
                var error = "OAuth token endpoint failure: " + await Display(response);
                return;
            }

        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private static async Task<string> Display(HttpResponseMessage response)
        {
            var output = new StringBuilder();
            output.Append("Status: " + response.StatusCode + ";");
            output.Append("Headers: " + response.Headers.ToString() + ";");
            output.Append("Body: " + await response.Content.ReadAsStringAsync() + ";");
            return output.ToString();
        }
    }
}
