using Area.DAT;
using Area.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Area.Controllers
{
    public class GithubController : Controller
    {
        public ActionResult GithubConnection()
        {
            var clientId = "Iv1.81c9b324b18a4188";
            var client = new GitHubClient(new ProductHeaderValue("AreaNet"));
            var request = new OauthLoginRequest(clientId)
            {
                Scopes = {"user", "notifications"},
                RedirectUri = new Uri("http://localhost:5000/Github/GithubAuthorized")
            };
            var oauthLoginUrl = client.Oauth.GetGitHubLoginUrl(request);
            return Redirect(oauthLoginUrl.ToString());
        }

        public async Task<ActionResult> GithubAuthorized(string code, string state, [FromServices] AreaDbContext DB)
        {
            string username = HttpContext.Session.GetString("username");
            if (String.IsNullOrEmpty(username))
                return Redirect("http://localhost:5000");
            try {
                var clientId = "Iv1.81c9b324b18a4188";
                var clientSecret = "d480b95d45b1147557bf97f51f87d0f4f7f9c14a";
                var client = new GitHubClient(new ProductHeaderValue("AreaNet"));
                if (String.IsNullOrEmpty(code))
                    return Redirect("http://localhost:5000");
                var request = new OauthTokenRequest(clientId, clientSecret, code);
                var token = await client.Oauth.CreateAccessToken(request);
                DB.tokens.Add( new Models.Token {type="Github", username=username, value=token.AccessToken});
                DB.SaveChanges();
            }
            catch
            {
                Console.WriteLine("Error in GithubAuthorize");
                return Redirect("http://localhost:5000");
            }
            return Redirect("http://localhost:5000");
        }
    }
}