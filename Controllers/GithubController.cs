using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AngularBBS.GitHub;
using AngularBBS.Models;
using AngularBBS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NuGet.Protocol.Core.v3;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AngularBBS.Controllers
{
    [Authorize]
    [Route("api/github")]
    public class GithubController : Controller
    {
        private readonly string _accessToken;
        private readonly HttpClient _httpClient;

        public GithubController(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _accessToken = GithubConfig.GetToken(httpContextAccessor);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Angular-bbs");
        }
        
        [Route("token")]
        [HttpGet]
        public string GetToken()
        {
            //Todo: Remove this api on production.
            return _accessToken;
        }

        [Route("issues")]
        [HttpGet("{state}")]
        public async Task<string> GetIssuesAsync(string state = "open")
        {
            var url = GithubEndpoints.QAndAIssuesEndpint + "?state=" + state;
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await _httpClient.SendAsync(requestMessage, HttpContext.RequestAborted);
            response.EnsureSuccessStatusCode();
            var issues = await response.Content.ReadAsStringAsync();
            return issues;
        }
        
        [Route("user")]
        [HttpGet]
        public async Task<string> GetUserAsync()
        {
            //            await HttpContext.Authentication.ChallengeAsync("GitHub");
            var url = GithubEndpoints.UserEndpoint;

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("filter", "all");

            var response = await _httpClient.SendAsync(requestMessage, HttpContext.RequestAborted);
            response.EnsureSuccessStatusCode();
            var user = JObject.Parse(await response.Content.ReadAsStringAsync()).ToJson();
            return user;
        }
        
        [Route("new")]
        [HttpPost]
        public async Task<string> NewIssueAsync([FromBody] IssueViewModel issue)
        {
            var url = GithubEndpoints.QAndAIssuesEndpint;
            var response = await _httpClient.PostAsJsonAsync(url, new
            {
                title = issue.Title,
                body = issue.Body
            });
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
    }
}