using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AngularBBS.Models;
using AngularBBS.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NuGet.Protocol.Core.v3;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AngularBBS.Controllers
{
    [Route("api/github")]
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;

        public LoginController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // todo: Store token in cookies or local storage
//            if (string.IsNullOrEmpty(AccessToken.Token))
//                throw new Exception("No Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken.Token);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Angular-bbs");
        }

        [Route("token")]
        [HttpGet]
        public string GetToken()
        {
            // todo: Not to expose in production. For development test purpose only. 
            return AccessToken.Token;
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
            output.Append("Headers: " + response.Headers + ";");
            output.Append("Body: " + await response.Content.ReadAsStringAsync() + ";");
            return output.ToString();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [Route("issues")]
        [HttpGet]
        public async Task<string> GetIssuesAsync()
        {
            var url = "https://api.github.com/repos/angular-bbs/q-and-a/issues?state=all";
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            
            var response = await _httpClient.SendAsync(requestMessage, HttpContext.RequestAborted);
            response.EnsureSuccessStatusCode();
            var issues = await response.Content.ReadAsStringAsync();

            return issues;
        }

        // GET api/values
        [Route("user")]
        [HttpGet]
        public async Task<string> GetUserAsync()
        {
            //            await HttpContext.Authentication.ChallengeAsync("GitHub");
            var url = "https://api.github.com/user"; // + GitHubSecret.ClientId;

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("filter", "all");

            var response = await _httpClient.SendAsync(requestMessage, HttpContext.RequestAborted);
            response.EnsureSuccessStatusCode();
            var user = JObject.Parse(await response.Content.ReadAsStringAsync()).ToJson();
            return user;
        }

        // POST api/values
        [Route("new")]
        [HttpPost]
        public async Task<string> NewIssueAsync([FromBody] IssueViewModel issue)
        {
            
            var url = "https://api.github.com/repos/angular-bbs/q-and-a/issues";
            //Todo: test purpose only, get token from /api/github/token, then copy it to postman or fiddler for testing.
            if (string.IsNullOrEmpty(issue.Token))
            {
                throw new Exception("No token provided");
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", issue.Token);
            var response = await _httpClient.PostAsJsonAsync(url, new
            {
                title = issue.Title,
                body = issue.Body
            });
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return result;
            
        }

//        // PUT api/values/5
//        [HttpPut("")]
//        public async Task PutAsync()
//        {
//            //Trying to get an access token with the api using client id and secret, but not working!
//            var url = "https://api.github.com/authorizations/clients/" + GitHubSecret.ClientId;
//            var tokenRequestParameters = new Dictionary<string, string>
//            {
//                {"client_secret", GitHubSecret.Secret}
//            };
//
//            var requestContent = new FormUrlEncodedContent(tokenRequestParameters);
//
//            var requestMessage = new HttpRequestMessage(HttpMethod.Put, url);
//            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//            requestMessage.Headers.Add("User-Agent", "Angular-bbs");
//            requestMessage.Content = requestContent;
//
//            var response = await _httpClient.SendAsync(requestMessage, HttpContext.RequestAborted);
//            response.EnsureSuccessStatusCode();
//            if (response.IsSuccessStatusCode)
//            {
//                var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
//            }
//            else
//            {
//                var error = "OAuth token endpoint failure: " + await Display(response);
//            }
//        }
    }
}