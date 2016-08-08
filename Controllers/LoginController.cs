using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;

using AngularBBS.Services;
using AngularBBS.Models.GithubViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AngularBBS.Controllers
{
    [Route("api/login-from-github")]
    public class LoginController : Controller
    {
        private readonly GithubConfigService _githubConfig;
        public LoginController(GithubConfigService githubConfig)
        {
            this._githubConfig = githubConfig;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        async public Task<object> Post([FromBody] LoginViewModel data)
        {
            string token = this._githubConfig.secret;
            using (var client = new HttpClient())
            {
                var myData = new
                {
                    client_id = this._githubConfig.clientId,
                    client_secret = this._githubConfig.secret,
                    code = data.code,
                    redirect_uri = data.redirect_uri,
                    state = data.state,
                };
                var body = new StringContent(myData.ToString(), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://github.com/login/oauth/access_token", body );

                var responseString = await response.Content.ReadAsStringAsync();
                return new {name = "Superman"};
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
